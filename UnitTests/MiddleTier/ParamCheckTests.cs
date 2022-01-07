using CodeBase;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness.WebServices;
using System.Collections;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Xml.Serialization;

namespace UnitTests.MiddleTier.ParamCheck_Tests {
	///<summary>This class will use reflection to indicate what methods do not have the correct parameters within their RemotingRole check.</summary>
	[TestClass]
	public class ParamCheckTests:TestBase {
		private static HashSet<string> _setMiddleTierMethods=new HashSet<string>();

		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
			RunTestsAgainstMiddleTier(new ParamCheckMockIIS());
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		public void TearDownTest() {
			RevertMiddleTierSettingsIfNeeded();
		}

		///<summary>This method will execute only once, just after all tests in this class have run.
		///However, this method is not guaranteed to execute before starting another TestMethod from another TestClass.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
		}

		///<summary>This test looks at the assembly, OpenDentBusiness, and tests to see that all methods that call Meth.Get... are passing in
		///the correct number of parameters. Also, it tests to make sure functions without calls to Meth.Get... do not have crud or database
		///calls.</summary>
		[TestMethod]
		public void MiddleTier_ParametersTest() {
			List<string> listSClassNames=GetSClassNames();
			StringBuilder strBuilderNoRemotingRolesCheck=new StringBuilder();
			StringBuilder strBuilderNoGetCurrentMethod=new StringBuilder();
			StringBuilder strBuilderIncorrectParameters=new StringBuilder();
			StringBuilder strBuilderIncorrectParametersOrder=new StringBuilder();
			//Load assembly from file
			AssemblyDefinition asm=AssemblyDefinition.ReadAssembly(@"OpenDentBusiness.dll");
			//Get all methods from the assembly.
			//Ignore Meth. We do not want to check methods from this class.
			//Needs to have a body see we can look through its contents. Public and static.
			List<MethodDefinition> listAllMethods=asm.Modules
				.SelectMany(x => x.Types)
				.Where(x => x.Name!=typeof(Meth).Name && !x.FullName.StartsWith("OpenDentBusiness.Crud"))
				.SelectMany(x => x.Methods)
				.Where(x => x.HasBody
				&& x.IsPublic
				&& x.IsStatic 
				&& x.Name!=SymbolExtensions.GetMethodInfo(() => WebServiceTests.InvalidWebMethod()).Name)
				.ToList();
			foreach(MethodDefinition method in listAllMethods) {
				//The assembly level instructions.
				List<Instruction> listMethodInstructions=method.Body.Instructions.ToList();
				//Find the instruction that is the call to Meth.GetTable, Meth.GetObject, etc.
				Instruction methodCallInstructionMeth=GetInstructionCallingMeth(method);
				//Handle any methods that do not invoke a "Meth" method.
				if(methodCallInstructionMeth==null) {
					#region No DB calls Check
					//Skip any non s class methods. Not worth worrying about these classes because they are typically handled in strange fashions.
					//E.g. classes such as ConvertDatabasesX.cs will always have a remoting role check done prior to invocation.
					if(!ListTools.In(method.DeclaringType.Name.ToLower(),listSClassNames)) {
						continue;
					}
					//Known methods that follow the rules in their own way. No point in programming around all these. If we know they are 
					//checking roles somehow, skip it.
					if((method.DeclaringType.FullName==typeof(Security).FullName 
							&& method.Name==SymbolExtensions.GetMethodInfo(() => Security.LogInWeb("","","","",false)).Name)
						|| (method.DeclaringType.FullName==typeof(ReplicationServers).FullName 
							&& method.Name==SymbolExtensions.GetMethodInfo(() => ReplicationServers.ServerIsBlocked()).Name)
						|| (method.DeclaringType.FullName==typeof(Lans).FullName 
							&& method.Name==SymbolExtensions.GetMethodInfo(() => Lans.LoadTranslationsFromTextFile("")).Name))
					{
						continue;
					}
					//Check to see if there are any method calls to the Crud or to any method in Db.
					if(DoesMethodCallTheDatabase(method)) {
						strBuilderNoRemotingRolesCheck.AppendLine($"Class: {method.DeclaringType.Name}\r\nMethod: {method.Name}\r\n");
					}
					continue;
					#endregion 
				}
				//We know it calls a remoting roles method. To tell how many parameters are passed in, we need to look for the LDC (load) commands
				//that load the variables into memory before calling the method.
				int indexOfMethodCall=listMethodInstructions.IndexOf(methodCallInstructionMeth);
				//To narrow our search of instructions, we know the first variable passed in will start with a call to MethodBase.GetCurrentMethod().
				Instruction instructionCurrentMethod=listMethodInstructions.Where(x => x.Operand!=null && x.OpCode==OpCodes.Call)
					.Where(x => (x.Operand as MethodReference)!=null)
					.FirstOrDefault(x => ((MethodReference)x.Operand).Name==SymbolExtensions.GetMethodInfo(() => MethodBase.GetCurrentMethod()).Name);
				//Doesn't exist. Happens in a few cases that we can ignore on a case by case basis.
				if(instructionCurrentMethod==null) {
					if((method.DeclaringType.FullName==typeof(Cache).FullName //No call to get current method as the method info is passed in. Skip check
							&& method.Name==SymbolExtensions.GetMethodInfo(() => Cache.GetTableRemotelyIfNeeded(null,"")).Name)
						|| method.DeclaringType.FullName==typeof(Reports).FullName //Uses Meth.GetTableLow which does not require method info.
							&& method.Name==SymbolExtensions.GetMethodInfo(() => Reports.GetTable("")).Name)
					{
						continue;
					}
					strBuilderNoGetCurrentMethod.AppendLine($"Class: {method.DeclaringType.Name}\r\nMethod: {method.Name}\r\n");
					continue;
				}
				int indexOfCurrentMethod=listMethodInstructions.IndexOf(instructionCurrentMethod);
				//If a method is simply Method(params something) or Method(something[]), the machine code will likely be different. Generally when 
				//passing parameters into a method such as Meth.GetObject, it will construct a new object[] and load objects into it. However, if the 
				//only parameter it needs to pass in is already a param[], no new object[] needs to be constructed. It can simply call ldarg0 to load 
				//the parameter directly in. Because of this, no ldc is called when MethodBase.GetCurrentMethod is called either. The result will be 
				//placed on the stack already in order. However, there are exceptions such as if the parameter array is used in a lambda expression. 
				//Instead of trying to handle each special case, if there are any ldc we will use the instruction that sets the number
				//of items in the new object array. Otherwise, we will use ldarg if no ldc/ldnull instructions are used.
				bool useLdc=listMethodInstructions.GetRange(indexOfCurrentMethod,indexOfMethodCall-indexOfCurrentMethod)
					.Any(x => x.OpCode.Name.ToLower().StartsWith("ldc") || x.OpCode.Name.ToLower().StartsWith("ldnull"));
				//Start at 1 for the call to MethodBase.GetCurrentMethod.
				int parametersPassedIn=1;
				foreach(Instruction instruc in listMethodInstructions.GetRange(indexOfCurrentMethod,indexOfMethodCall-indexOfCurrentMethod)) {
					if(!useLdc) {
						if(instruc.OpCode.Name.ToLower().StartsWith("ldarg")) {
							parametersPassedIn++;
						}
						continue;
					}
					//When looking at ldc, this generally means that a new object[] is being created for the params.
					//Before making the array, an integer is loaded onto the stack with the number of items in the array.
					//We need to look as this number as when we pass in a null as a parameter, no extra instructions are created
					//or changed except for pushing a higher number on the stack. 
					//A special case is a single null passed in as the params. The only thing it will have is a ldnull command.
					if(instruc.OpCode.Name.ToLower().StartsWith("ldc.i4") || instruc.OpCode.Name.ToLower().StartsWith("ldnull")) {
						//Once we find the very first instance of this, we know that this is for the new object array that is being created.
						//There are many short hand instructions for loading a specific number such as ldc_i4_1 ldc_i4_1. If we are loading
						//more than 8 arguments, we will use the ldc_i4 or ldc_i4_s instruction. In this case we will set the number as the operand.
						switch(instruc.OpCode.Code) {
							case Code.Ldc_I4_0:
								parametersPassedIn+=0;
								break;
							case Code.Ldc_I4_1:
							case Code.Ldnull:
								parametersPassedIn+=1;
								break;
							case Code.Ldc_I4_2:
								parametersPassedIn+=2;
								break;
							case Code.Ldc_I4_3:
								parametersPassedIn+=3;
								break;
							case Code.Ldc_I4_4:
								parametersPassedIn+=4;
								break;
							case Code.Ldc_I4_5:
								parametersPassedIn+=5;
								break;
							case Code.Ldc_I4_6:
								parametersPassedIn+=6;
								break;
							case Code.Ldc_I4_7:
								parametersPassedIn+=7;
								break;
							case Code.Ldc_I4_8:
								parametersPassedIn+=8;
								break;
							case Code.Ldc_I4_S:
								if(instruc.Operand==null) {
									Assert.Fail($"Operand is null for {instruc.OpCode.Code.ToString()}");
								}
								parametersPassedIn+=(sbyte)instruc.Operand;
								break;
							case Code.Ldc_I4:
								if(instruc.Operand==null) {
									Assert.Fail($"Operand is null for {instruc.OpCode.Code.ToString()}");
								}
								parametersPassedIn+=(int)instruc.Operand;
								break;
							default:
								Assert.Fail($"An unhandled instruction of {instruc.OpCode.Code.ToString()}");
								break;
						}
						//Since we made it here, we know how many parameters were passed in.
						break;
					}
				}
				if(parametersPassedIn-1!=method.Parameters.Count) {
					strBuilderIncorrectParameters.AppendLine($"Class: {method.DeclaringType.Name}\r\nMethod: {method.Name}\r\n");
				}
				#region Parameter Order
				//Known methods that follow the rules in their own way. No point in programming around all these. If we know they are 
				//checking the parameter order somehow, skip it.
				if((method.DeclaringType.FullName==typeof(Fees).FullName
						&& method.Name==SymbolExtensions.GetMethodInfo(() => Fees.GetByFeeSchedNumsClinicNums(null,null)).Name)
					|| (method.DeclaringType.FullName==typeof(Schedules).FullName
						&& method.Name==SymbolExtensions.GetMethodInfo(() => Schedules.Insert(false,false)).Name)
					|| (method.DeclaringType.FullName==typeof(Statements).FullName
						&& method.Name==SymbolExtensions.GetMethodInfo(() => Statements.InsertMany(null)).Name)) 
				{
					continue;
				}
				//Make sure that each parameter used within the Meth method call utilizes the parameters in the correct order as they were passed in.
				//E.g. A method that takes multiple parameters of the same type needs to make sure that the order of these parameters is preserved.
				int indexOfParamOrder=indexOfCurrentMethod;
				if(useLdc) {
					indexOfParamOrder=listMethodInstructions.IndexOf(
						listMethodInstructions.GetRange(indexOfCurrentMethod,indexOfMethodCall-indexOfCurrentMethod)
							.First(x => x.OpCode.Name.ToLower().StartsWith("ldc") || x.OpCode.Name.ToLower().StartsWith("ldnull")));
					//Start at the instruction directly after the declaration of the usage of a ldc new object[] for the params.
					indexOfParamOrder++;
				}
				int index=0;
				string loadInstructStr=(useLdc ? "ldc.i4" : "ldarg");
				foreach(Instruction instruc in listMethodInstructions.GetRange(indexOfParamOrder,indexOfMethodCall-indexOfParamOrder)) {
					string opCodeName=instruc.OpCode.Name.ToLower();
					if(!opCodeName.StartsWith(loadInstructStr)) {
						continue;
					}
					string actualInstructStr=instruc.ToString();//We have to use .ToString() for methods with more than 8 parameters.
					string expectedInstructStr;
					if(useLdc && opCodeName==$"{loadInstructStr}.s") {
						expectedInstructStr=$"{loadInstructStr}.s {index}";
					}
					else {
						expectedInstructStr=$"{loadInstructStr}.{index}";
					}
					//We have to use .EndsWith() for methods with more than 8 parameters.
					if(!actualInstructStr.EndsWith(expectedInstructStr)) {
						strBuilderIncorrectParametersOrder.AppendLine($"Class: {method.DeclaringType.Name}\r\nMethod: {method.Name}\r\n");
						break;
					}
					if(useLdc) {
						Instruction instructionNext=instruc.Next;
						string opCodeNameNext=instructionNext.OpCode.Name.ToLower();
						if(opCodeNameNext=="ldarg.s") {
							if(!instructionNext.ToString().ToLower().EndsWith($"ldarg.s {method.Parameters[index].Name.ToLower()}")) {
								strBuilderIncorrectParametersOrder.AppendLine($"Class: {method.DeclaringType.Name}\r\nMethod: {method.Name}\r\n");
								break;
							}
						}
						else if(opCodeNameNext=="ldloc.0" && instructionNext.OpCode.OperandType==OperandType.InlineNone) {
							//Ignored parameter.
							//E.g. The calling method didn't specify the parameter and that's fine because the parameter is optional OR is a "params".
						}
						else if(instruc.Next.OpCode.Name.ToLower()!=$"ldarg.{index}") {
							strBuilderIncorrectParametersOrder.AppendLine($"Class: {method.DeclaringType.Name}\r\nMethod: {method.Name}\r\n");
							break;
						}
					}
					index++;
				}
				#endregion
			}
			List<string> listErrors=new List<string>();
			if(strBuilderNoRemotingRolesCheck.Length!=0) {
				listErrors.Add("The following methods call the database (crud/db/datacore) without a remoting roles check.\r\n"
					+strBuilderNoRemotingRolesCheck.ToString());
			}
			if(strBuilderNoGetCurrentMethod.Length!=0) {
				listErrors.Add("In the following methods, there was no call to MethodBase.GetCurrentMethod().\r\n"
					+strBuilderNoGetCurrentMethod.ToString());
			}
			if(strBuilderIncorrectParameters.Length!=0) {
				listErrors.Add("In the following methods, there was a mismatching number of middle tier parameters.\r\n"
					+strBuilderIncorrectParameters.ToString());
			}
			if(strBuilderIncorrectParametersOrder.Length!=0) {
				listErrors.Add("In the following methods, the order of the parameters passed into the method are not preserved correctly in the Meth call.\r\n"
					+strBuilderIncorrectParametersOrder.ToString());
			}
			Assert.AreEqual(0,listErrors.Count,"ParamCheckTest ERRORS:\r\n"
				+string.Join("============================================================\r\n",listErrors));
		}

		///<summary>This test ensures that all public static s class methods that return void have a return within their middle tier check.
		///For example,
		///if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
		///		Meth.GetVoid(MethodBase.GetCurrentMethod());
		///		return;
		///}
		///</summary>
		[TestMethod]
		public void MiddleTier_ReturnAfterVoid() {
			StringBuilder strBuilderMissingReturn=new StringBuilder();
			List<string> listSClassNames=GetSClassNames();
			//Load assembly from file
			AssemblyDefinition asm=AssemblyDefinition.ReadAssembly(@"OpenDentBusiness.dll");
			//Get all methods from the assembly.
			List<MethodDefinition> listAllSClassMethods=asm.Modules
				.SelectMany(x => x.Types)
				.Where(x => ListTools.In(x.Name.ToLower(),listSClassNames))
				.SelectMany(x => x.Methods)
				.Where(x => x.HasBody && x.IsPublic && x.IsStatic)//Needs to have a body see we can look through its contents. Public and static.
				.ToList();
			foreach(MethodDefinition method in listAllSClassMethods) {
				//The assembly level instructions.
				List<Instruction> listMethodInstructions=method.Body.Instructions.ToList();
				//Instructions that call a method.
				List<Instruction> listMethodCallInstructions=listMethodInstructions.Where(x => x.Operand!=null && x.OpCode==OpCodes.Call)
					.Where(x => (x.Operand as MethodDefinition)!=null).ToList();
				//Find the instruction that is the call to Meth.GetVoid
				Instruction methodCallInstructionVoid=listMethodCallInstructions
					.FirstOrDefault(x => ((MethodDefinition)x.Operand).DeclaringType.FullName==typeof(Meth).FullName
					&& ((MethodDefinition)x.Operand).Name==SymbolExtensions.GetMethodInfo(() => Meth.GetVoid(null)).Name);
				//Does not call GetVoid.
				if(methodCallInstructionVoid==null) {
					continue;
				}
				//Now we need to figure out if there is a call to return in the following if statement
				//if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) { }
				//To do this, we can find the instruction where the if begins (RemotingClient.RemotingRole) and the instruction where the if ends. 
				//If we look at the instructions in between the two, we can see if 'ret' is called or if we branch to the ret instruction.
				Instruction getRemotingRoleInstruction=listMethodCallInstructions.FirstOrDefault(x => 
					((MethodDefinition)x.Operand).DeclaringType.FullName==typeof(RemotingClient).FullName
					&& ((MethodDefinition)x.Operand).Name=="get_RemotingRole");
				if(getRemotingRoleInstruction==null) {
					throw new Exception("Cannot find get remoting roles.");
				}
				int indexOfRemotingRoleInstruction=listMethodInstructions.IndexOf(getRemotingRoleInstruction);
				//To figure out where the if ends, we need to find the branch if false command and what instruction it leads to. We will look for 
				//the first command starting after the instruction above.
				Instruction branchIfFalseInstruction=listMethodInstructions.GetRange(indexOfRemotingRoleInstruction,
					listMethodInstructions.Count-indexOfRemotingRoleInstruction)
					.FirstOrDefault(x => ListTools.In(x.OpCode,OpCodes.Brfalse,OpCodes.Brfalse_S));
				if(branchIfFalseInstruction==null) {
					throw new Exception("Cannot find branch if false instruction.");
				}
				int indexOfBranchIfFalse=listMethodInstructions.IndexOf(branchIfFalseInstruction);
				//The operand of a branch is the instruction it will branch to
				Instruction branchDestination=(Instruction)branchIfFalseInstruction.Operand;
				int indexOfEndOfIf=listMethodInstructions.IndexOf(branchDestination);
				bool doesMethodReturn=false;
				foreach(Instruction instruction in listMethodInstructions.GetRange(indexOfBranchIfFalse,indexOfEndOfIf-indexOfBranchIfFalse)) {
					if(instruction.OpCode==OpCodes.Ret) {
						//The return command.
						doesMethodReturn=true;
					}
					else if(ListTools.In(instruction.OpCode,OpCodes.Br_S,OpCodes.Br)) {
						//This is a branch. Let's see if the instruction that it is branching to is a return command.
						Instruction branchInstruction=(Instruction)instruction.Operand;
						if(branchInstruction.OpCode==OpCodes.Ret) {
							doesMethodReturn=true;
						}
					}
				}
				if(!doesMethodReturn) {
					strBuilderMissingReturn.AppendLine($"Class: {method.DeclaringType.Name}\r\nMethod: {method.Name}\r\n");
				}
			}
			if(strBuilderMissingReturn.Length!=0) {
				Console.WriteLine("The following methods have calls to Meth.GetVoid that do not return after the middle tier call has completed.");
				Console.WriteLine(strBuilderMissingReturn.ToString());
			}
			Assert.AreEqual(0,strBuilderMissingReturn.Length);
		}

		///<summary>This test ensures that there are NO private static methods that call Meth.Get... . If this occurs, the request will get to the
		///middle tier and then an exception will be thrown as the middle tier server cannot find the method.</summary>
		[TestMethod]
		public void MiddleTier_NoPrivateMeth() {
			StringBuilder strBuilderPrivateMeth=new StringBuilder();
			List<string> listSClassNames=GetSClassNames();
			//Load assembly from file
			AssemblyDefinition asm=AssemblyDefinition.ReadAssembly(@"OpenDentBusiness.dll");
			//Get all methods from the assembly.
			List<MethodDefinition> listAllSClassMethods=asm.Modules
				.SelectMany(x => x.Types)
				.Where(x => ListTools.In(x.Name.ToLower(),listSClassNames))
				.SelectMany(x => x.Methods)
				.Where(x => x.HasBody && !x.IsPublic && x.IsStatic)//Needs to have a body see we can look through its contents. Non-public and static.
				.ToList();
			foreach(MethodDefinition method in listAllSClassMethods) {
				Instruction instructionMethodCall=GetInstructionCallingMeth(method);
				//If this is not null, our pattern is broken and someone is trying to call Meth.Get... in a private method.
				if(instructionMethodCall!=null) {
					strBuilderPrivateMeth.AppendLine($"Class: {method.DeclaringType.Name}\r\nMethod: {method.Name}\r\n");
				}
			}
			if(strBuilderPrivateMeth.Length!=0) {
				Console.WriteLine("The following methods have calls to Meth.Get... but are private. They should be public, reworked, or the remoting "
					+"check should be removed.");
				Console.WriteLine(strBuilderPrivateMeth.ToString());
			}
			Assert.AreEqual(0,strBuilderPrivateMeth.Length);
		}

		///<summary>The following test looks at OpenDentBusiness. It finds all database calls in private static methods in our S classes, which by
		///nature have no remoting roles check. Then, it looks at all references to these methods. It will trace its way up each potential
		///call stack until it reaches a public static method. If it has a remoting roles check, on to the next one. Otherwise, there is a path the
		///code could take to reach methods that are not behind a remoting roles check. Other tests ensure public methods have remoting roles checks
		///in the case of database calls directly in that method. Also, other tests ensure that private static methods do not have an erroneous remoting
		///roles check.</summary>
		[TestMethod]
		public void MiddleTier_PathToError() {
			StringBuilder strBuilderBadPaths=new StringBuilder();
			List<string> listSClassNames=GetSClassNames();
			//Load assembly from file
			AssemblyDefinition asm=AssemblyDefinition.ReadAssembly(@"OpenDentBusiness.dll");
			List<TypeDefinition> listSClassTypes=asm.Modules
				.SelectMany(x => x.Types)
				.Where(x => ListTools.In(x.Name.ToLower(),listSClassNames))
				.ToList();
			//This is a dictionary where the key is the method definition of a method. The value is a list of methods that call the key method. 
			//Only fills static methods.
			Dictionary<MethodDefinition,List<MethodDefinition>> dictMethodNameToMethodReferences=FillDictOfMethodReferences(listSClassTypes);
			//Get all methods from the assembly.
			List<MethodDefinition> listPrivateSClassMethodsWithDbCall=listSClassTypes
				.SelectMany(x => x.Methods)
				.Where(x => x.HasBody && !x.IsPublic && x.IsStatic && DoesMethodCallTheDatabase(x))//Needs to have a body see we can look through its contents. Non-public and static and makes a database call.
				.ToList();
			foreach(MethodDefinition method in listPrivateSClassMethodsWithDbCall) {
				CheckReferencesForMethod(method,strBuilderBadPaths,new List<MethodDefinition>(),dictMethodNameToMethodReferences);
			}
			if(strBuilderBadPaths.Length!=0) {
				Console.WriteLine("The following paths can cause an error. There are no remoting roles check along the path. There is a database call "
					+"along the path, however.");
				Console.WriteLine(strBuilderBadPaths.ToString());
			}
			Assert.AreEqual(0,strBuilderBadPaths.Length);
		}

		[TestMethod]
		public void ParamCheckSClasses() {
			if(DateTime.Now.Year > 1970) {
				return;//This test is a bit dangerous, so it does not run by default. Comment out this line to run.
			}
			StringBuilder retVal=new StringBuilder();
			List<string> arraySClassPaths=Directory.GetFiles(@"C:\Development\OPEN DENTAL SUBVERSION\head\OpenDentBusiness\Data Interface").ToList();
			arraySClassPaths.AddRange(Directory.GetFiles(@"C:\Development\OPEN DENTAL SUBVERSION\head\OpenDentBusiness\Db Multi Table"));
			//Get just the names of the classes without the path and extension information.
			List<string> listSClassNames=arraySClassPaths.Select(x => Path.GetFileNameWithoutExtension(x)).ToList();
			Assembly ass=Assembly.LoadFrom(@"C:\Development\OPEN DENTAL SUBVERSION\head\UnitTests\bin\Debug\OpenDentBusiness.dll");
			List<Type> listSClassTypes=ass.GetTypes().Where(x => x.IsClass && x.IsPublic && listSClassNames.Contains(x.Name))
				//.Where(x => x.Name=="AccountModules")
				.OrderBy(x => x.Name)
				.ToList();
			//Loop through all classes and call every public static method at least once.
			//There will be a log created for all methods that end up calling Meth.Dto... which are going to be the only methods we really care about.
			foreach(Type sClass in listSClassTypes) {
				MethodInfo[] methods=sClass.GetMethods(BindingFlags.Static | BindingFlags.Public);
				foreach(MethodInfo method in methods) {
					if(method.IsGenericMethod || method.ContainsGenericParameters || method.IsAbstract || method.IsConstructor || method.IsFinal 
						|| method.IsVirtual) 
					{
						continue;
					}
					if(method.Name.StartsWith("get_") || method.Name.StartsWith("set_")
						//Cache methods
						|| ListTools.In(method.Name,"GetFirstOrDefault","GetWhere","GetExists","GetExists","GetLastOrDefault","GetLast","GetFindIndex","GetFirst",
							"GetFirstOrDefaultFromList","GetWhereFromList","FillCacheFromTable")
						//Methods that are too hard to invoke
						|| (sClass.Name=="Signalods" && method.Name=="SignalsTick")
						|| (sClass.Name=="Signalods" && method.Name=="SubscribeSignalProcessor")
						|| (sClass.Name=="MarkupEdit" && method.Name=="ValidateNodes")
						|| (sClass.Name=="MarkupEdit" && method.Name=="ValidateDuplicateNesting")
						//Methods that are too dangerous to run
						|| (sClass.Name=="MiscData" && method.Name=="SetMaxAllowedPacket")
						//Methods that cause a UI to come up
						|| (sClass.Name=="Security" && method.Name=="IsAuthorized")) 
					{
						continue;
					}
					bool hasOutOrRefParam=false;
					ParameterInfo[] parameters=method.GetParameters();
					object[] arrayObjs=ConstructParameters(parameters,sClass,method,retVal,out hasOutOrRefParam);					
					if(hasOutOrRefParam) {
						continue;//Middle tier does not support out or ref params.
					}
					try {
						method.Invoke(null,arrayObjs);
					}
					catch(Exception ex) {
						//Check that the error message comes from a middle tier bug
						ex=MiscUtils.GetExceptionContainingMessage(ex,"Method not found with ")
							??MiscUtils.GetExceptionContainingMessage(ex,"Middle Tier, incorrect number of parameters")
							??MiscUtils.GetExceptionContainingMessage(ex,"does not have a parameterless constructor.")
							??MiscUtils.GetExceptionContainingMessage(ex,"serializ")
							??MiscUtils.GetExceptionContainingMessage(ex,"There was an error generating the XML document")
							??MiscUtils.GetExceptionContainingMessage(ex,"There is an error in XML document")
							??MiscUtils.GetExceptionContainingMessage(ex,"No longer allowed to send SQL directly.")
							??MiscUtils.GetExceptionContainingMessage(ex,"Not allowed to send sql directly.");
						if(ex==null) {
							continue;
						}
						string error="Method: "+sClass?.Name+"."+method?.Name+"\r\n"
							+"\tError: "+ex.Message;
						Console.WriteLine(error);
						retVal.AppendLine(error);
					}
				}
			}
			Assert.AreEqual("",retVal.ToString());
		}

		///<summary>Creates an array of parameters with the default value for each parameter.</summary>
		public static object[] ConstructParameters(ParameterInfo[] parameters,Type sClass,MethodInfo method,StringBuilder sbErrors,out bool hasOutOrRefParam) {
			object[] arrayObjs=new object[parameters.Length];
			hasOutOrRefParam=false;
			for(int i=0;i<parameters.Length;i++) {
				try {
					Type parameterType=parameters[i].ParameterType;
					if(parameterType.IsByRef) {
						hasOutOrRefParam=true;
						break;
					}
					if(parameterType==typeof(string)) {
						arrayObjs[i]="";
					}
					else if(parameterType.IsArray) {
						arrayObjs[i]=Array.CreateInstance(parameterType.GetElementType(),0);
					}
					else if(parameterType==typeof(DataRow)) {
						arrayObjs[i]=new DataTable().NewRow();
					}
					else if(parameterType==typeof(DateTime)) {
						arrayObjs[i]=DateTime.Now;
					}
					else if(parameterType==typeof(Version)) {
						arrayObjs[i]=new Version(1,1,1,1);//Versions cannot be 0.0 for JSON
					}
					else if(parameterType==typeof(CultureInfo)) {
						arrayObjs[i]=CultureInfo.CurrentCulture;
					}
					else if(parameterType==typeof(CodeSystems.ProgressArgs)) {
						arrayObjs[i]=new CodeSystems.ProgressArgs((Action<int,int>)delegate (int a,int b) { });
					}
					else if(parameterType==typeof(Logger.WriteLineDelegate)) {
						arrayObjs[i]=new Logger.WriteLineDelegate((Action<string,LogLevel>)delegate (string a,LogLevel b) { });
					}
					else if(parameterType==typeof(Logger.IWriteLine)) {
						arrayObjs[i]=new LogDelegate(new Logger.WriteLineDelegate((a,b) => { }));
					}
					else if(parameterType==typeof(IODProgress)) {
						arrayObjs[i]=new ODProgressDoNothing();
					}
					else if(parameterType==typeof(IODProgressExtended)) {
						arrayObjs[i]=new ODProgressExtendedNull();
					}
					else if(parameterType==typeof(MethodInfo)) {
						arrayObjs[i]=method;
					}
					else if(parameterType.IsEnum) {
						foreach(var enumVal in Enum.GetValues(parameterType)) {
							arrayObjs[i]=enumVal;
							break;
						}
					}
					else {
						arrayObjs[i]=Activator.CreateInstance(parameterType);
						if(!parameterType.IsValueType) {
							//A class type
							FieldInfo[] fiArray=parameterType.GetFields().Where(x => x!=null).ToArray();
							foreach(FieldInfo fi in fiArray) {
								object objCur=fi.GetValue(arrayObjs[i]);
								if(objCur==null) {
									continue;
								}
								if(fi.FieldType.IsEnum) {
									foreach(var enumVal in Enum.GetValues(fi.FieldType)) {
										objCur=enumVal;
										fi.SetValue(arrayObjs[i],objCur);
										break;
									}
								}
							}
						}
					}
				}
				catch(Exception ex) {
					ex=MiscUtils.GetInnermostException(ex);
					string error="!!!!!UNSUPPORTED PARAMETER TYPE!!!!\r\n"
								+"Method: "+sClass?.Name+"."+method?.Name+"\r\n"
								+"\tError: "+ex.Message;
					sbErrors.AppendLine(error);
				}
			}
			return arrayObjs;
		}

		[TestMethod]
		public void NoDataTableOrDataSetFields() {
			if(_setMiddleTierMethods.Count==0) {
				ParamCheckSClasses();//This method will populate the list of Middle Tier methods.
			}
			foreach(string fullMethodName in _setMiddleTierMethods) {
				string[] fullNameComponents=fullMethodName.Split('.');
				string assemblyName=fullNameComponents[0];//OpenDentBusiness
				string className=fullNameComponents[1];
				string methodName=fullNameComponents[2];
				Type classType=Type.GetType(assemblyName//actually, the namespace which we require to be same as assembly by convention
					+"."+className+","+assemblyName);
				MethodInfo[] methods=classType.GetMethods(BindingFlags.Public|BindingFlags.Static)
					.Where(x => x.Name==methodName).ToArray();
				Type[] parameters=methods.SelectMany(x => x.GetParameters()).Select(x => x.ParameterType).ToArray();
				Assert.IsFalse(parameters.Any(x => HasDataTableOrSetField(x)),"Method "+fullMethodName+" has a parameter that contains "
					+" a DataTable/Set.");
				Type[] returnTypes=methods.Select(x => x.ReturnType).ToArray();
				Assert.IsFalse(returnTypes.Any(x => HasDataTableOrSetField(x)),"Method "+fullMethodName+" has a return type that contains a DataTable/Set.");
			}
		}

		private bool HasDataTableOrSetField(Type type) {
			if(type==typeof(DataTable) || type==typeof(DataSet)) {
				return false;
			}
			return HasDataTableOrSetFieldRecursive(type,0);
		}

		private bool HasDataTableOrSetFieldRecursive(Type type,int depth) {
			if(depth==50) {
				return false;//In case an object contains itself as a field.
			}
			if(type==typeof(string)) {
				return false;
			}
			if(type.IsValueType) {
				return false;
			}
			if(type==typeof(DataTable) || type==typeof(DataSet)) {
				return true;
			}
			if(type.Namespace.StartsWith("System")) {
				return false;
			}
			if(type.IsGenericType) {
				Type[] genericTypes=type.GetGenericArguments();
				int newDepth=depth+1;
				return genericTypes.Any(x => HasDataTableOrSetFieldRecursive(x,newDepth));
			}
			if(typeof(IEnumerable).IsAssignableFrom(type)) {//type is probably an array
				Type elementType=type.GetElementType();
				if(elementType==null) {
					throw new Exception("Unsupported type: "+type.Name);
				}
				return HasDataTableOrSetFieldRecursive(elementType,++depth);
			}
			else {
				//if the object is not a value type and is not in the System namespace (besides strings, lists, and arrays) then it must be a class object,
				//i.e. a Patient or an Appointment object
				Type[] fieldTypes=type.GetFields(BindingFlags.Public|BindingFlags.Instance).Where(x => x!=null)
					.Where(x => x.GetCustomAttributes<XmlIgnoreAttribute>().Count()==0)
					.Select(x => x.FieldType).ToArray();
				int newDepth=depth+1;
				if(fieldTypes.Any(x => HasDataTableOrSetFieldRecursive(x,newDepth))) {
					return true;
				}
				Type[] propTypes=type.GetProperties().Where(x => x!=null)
					.Where(x => x.GetCustomAttributes<XmlIgnoreAttribute>().Count()==0)
					.Select(x => x.PropertyType).ToArray();
				if(propTypes.Any(x => HasDataTableOrSetFieldRecursive(x,newDepth))) {
					return true;
				}
				return false;
			}
		}

		///<summary>Checks the given method to find if it is a bad path. First checks to see if the method is public static. If it is, and it does not
		///call a remoting roles check, this is an error. If it is private, it will continue up the potential path via references to the method
		///by recursively invoking this method until a valid method has been found.</summary>
		///<param name="method">The current method being checked.</param>
		///<param name="strBuilderBadPaths">The string builder that will store the errors is one occurs.</param>
		///<param name="listCurrentPath">The list of method definitions that is the path to get to the current method passed in.</param>
		///<param name="dictReferences">A dictionary that contains a method as a key and all its references as the value.</param>
		private static void CheckReferencesForMethod(MethodDefinition method,StringBuilder strBuilderBadPaths,List<MethodDefinition> listCurrentPath,
			Dictionary<MethodDefinition,List<MethodDefinition>> dictReferences) 
		{
			if(method.IsPublic) {
				if(GetInstructionCallingMeth(method)!=null) {
					//This means the method has a remoting roles check. This is a valid path. Nothing needs to be done.
					return;
				}
				//Otherwise this is a public method that has a path to a private method that calls the database. This is an error. The path is in reverse
				//order as passed in the list.
				string error=$"{method.DeclaringType.Name}.{method.Name} -> ";
				for(int i=listCurrentPath.Count-1;i>=0;i--) {
					error+=$"{listCurrentPath[i].DeclaringType.Name}.{listCurrentPath[i].Name}";
					if(i!=0) {
						error+=" -> ";
					}
				}
				strBuilderBadPaths.AppendLine(error);
			}
			else {
				List<MethodDefinition> listMethodsThatReferencesThis;
				if(!dictReferences.TryGetValue(method,out listMethodsThatReferencesThis)) {
					//No references to this non-public method, skip.
					return;
				}
				//The method is not public, and it has references. We need to continue our way up the callstack to see if there is a path that could 
				//slip through.
				foreach(MethodDefinition methodReferencesThis in listMethodsThatReferencesThis) {
					//If the current path already contains the method reference, there is some sort of loop. We only need to check each method and its
					//references once. Continue on.
					if(listCurrentPath.Contains(methodReferencesThis)) {
						continue;
					}
					//Create a new list based on the path passed in. Append our current method to the path.
					List<MethodDefinition> listMethodPath=new List<MethodDefinition>(listCurrentPath);
					listMethodPath.Add(method);
					CheckReferencesForMethod(methodReferencesThis,strBuilderBadPaths,listMethodPath,dictReferences);
				}
			}
		}

		///<summary>Returns a dictionary where the key is the method definition of a method. The value is a list of methods that call the key method.</summary>
		private static Dictionary<MethodDefinition,List<MethodDefinition>> FillDictOfMethodReferences(List<TypeDefinition> listSClassTypes) {
			Dictionary<MethodDefinition,List<MethodDefinition>> dict=new Dictionary<MethodDefinition,List<MethodDefinition>>();
			//Find all static methods.
			List<MethodDefinition> listMethods=listSClassTypes
				.SelectMany(x => x.Methods)
				.Where(x => x.HasBody)
				.ToList();
			foreach(MethodDefinition method in listMethods) {
				List<Instruction> listInstructions=method.Body.Instructions.ToList();
				//Find instructions that are references to other methods and add them to the dictionary
				foreach(Instruction instruc in listInstructions) {
					//This is not a call command or a method reference. Skip.
					if(instruc.OpCode!=OpCodes.Call
						|| instruc.Operand==null
						|| !(instruc.Operand is MethodReference)) 
					{
						continue;
					}
					//The reference to the method.
					MethodReference methodRefBeingCalled=(MethodReference)instruc.Operand;
					if(methodRefBeingCalled.FullName==method.FullName) {
						//The method calls itself. Don't include this as it is not necessary for our uses. It will also cause an infinite loop.
						continue;
					}
					MethodDefinition methodDefBeingCalled=null;
					ODException.SwallowAnyException(() => methodDefBeingCalled=methodRefBeingCalled.Resolve());
					if(methodDefBeingCalled==null) {
						//Couldn't resolve. Skip it.
						continue;
					}
					List<MethodDefinition> listMethodReferences;
					if(!dict.TryGetValue(methodDefBeingCalled,out listMethodReferences)) {
						listMethodReferences=new List<MethodDefinition>();
						//Does not exist in dictionary. Create its entry
						dict[methodDefBeingCalled]=listMethodReferences;
					}
					//Add only if it is not already in the list.
					if(!listMethodReferences.Contains(method)) {
						listMethodReferences.Add(method);
					}
				}
			}
			return dict;
		}

		///<summary>Returns a list of strings that represent the names of the S classes in OpenDentBusiness. The names are all lower case.</summary>
		private static List<string> GetSClassNames() {
			List<string> arraySClassPaths=Directory.GetFiles(@"..\..\..\OpenDentBusiness\Data Interface").ToList();
			arraySClassPaths.AddRange(Directory.GetFiles(@"..\..\..\OpenDentBusiness\Db Multi Table"));
			//Get just the names of the classes without the path and extension information.
			return arraySClassPaths.Select(x => Path.GetFileNameWithoutExtension(x).ToLower()).ToList();
		}

		///<summary>Returns the instruction from the method that is the call to any Meth.Get... method. If the method does not have a body or the 
		///instruction could not be found, will return null.</summary>
		private static Instruction GetInstructionCallingMeth(MethodDefinition method) {
			if(!method.HasBody) {
				return null;
			}
			List<Instruction> listInstructions=method.Body.Instructions.ToList();
			Instruction methodCallInstructionMeth=listInstructions.Where(x => x.Operand!=null && x.OpCode==OpCodes.Call)
				.Where(x => (x.Operand as MethodDefinition)!=null)
				.FirstOrDefault(x => ((MethodDefinition)x.Operand).DeclaringType.FullName==typeof(Meth).FullName);
			if(methodCallInstructionMeth==null) {
				//Specifically for Meth.GetObject<T>, the Operand is a GenericInstanceMethod instead of MethodDefintion. Check for this as well.
				methodCallInstructionMeth=listInstructions.Where(x => x.Operand!=null && x.OpCode==OpCodes.Call)
					.Where(x => (x.Operand as GenericInstanceMethod)!=null)
					.FirstOrDefault(x => ((GenericInstanceMethod)x.Operand).DeclaringType.FullName==typeof(Meth).FullName);
			}
			return methodCallInstructionMeth;
		}
		

		///<summary>Returns if the passed in method makes a database call that would need to be wrapped in a middle tier check.</summary>
		private static bool DoesMethodCallTheDatabase(MethodDefinition method) {
			//Check to see if there are any method calls to the Crud or to any method in Db. These all need to be wrapped in a remoting roles
			//check.
			List<Instruction> listCallingInstructions=method.Body.Instructions.Where(x => x.Operand!=null && x.OpCode==OpCodes.Call)
				.Where(x => (x.Operand as MethodDefinition)!=null).ToList();
			bool isCrudCalled=listCallingInstructions
				.Any(x => ((MethodDefinition)x.Operand).DeclaringType.FullName.StartsWith("OpenDentBusiness.Crud") 
				&& !ListTools.In(((MethodDefinition)x.Operand).Name,"TableToList","ListToTable","UpdateComparison"));//Skip these as they do not call the db.
			if(isCrudCalled) {
				//Skip the db check to speed up the process if we know the method already calls the crud.
				return true;
			}
			return listCallingInstructions.Any(x => ListTools.In(((MethodDefinition)x.Operand).DeclaringType.Name,typeof(Db).Name,typeof(DataCore).Name));
		}

		private class ParamCheckMockIIS : OpenDentalServerMockIIS, IOpenDentalServer {
			public new string ProcessRequest(string dtoString) {
				DataTransferObject dto=DataTransferObject.Deserialize(dtoString);
				_setMiddleTierMethods.Add(dto.MethodName);
				return base.ProcessRequest(dtoString);
			}
		}
	}
}
