using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;
using Mono.Cecil.Cil;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UnitTests.MiddleTier.ParamCheck_Tests {
	///<summary>This class will use reflection to indicate what methods do not have the correct parameters within their MiddleTierRole check.</summary>
	[TestClass]
	public class ParamCheckTests:TestBase {

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
			RunTestsAgainstMiddleTier(new OpenDentBusiness.WebServices.OpenDentalServerMockIIS());
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		public void TearDownTest() {
			RevertMiddleTierSettingsIfNeeded();
		}

		///<summary>This test ensures all methods that call Meth.Get... are passing the correct number of parameters.
		///It also ensures functions without calls to Meth.Get... do not make CRUD or database calls.</summary>
		[TestMethod]
		public void MiddleTier_ParametersTest() {
			List<string> listSClassNames=GetSClassNames();
			StringBuilder strBuilderNoMiddleTierRolesCheck=new StringBuilder();
			StringBuilder strBuilderNoGetCurrentMethod=new StringBuilder();
			StringBuilder strBuilderIncorrectParameters=new StringBuilder();
			StringBuilder strBuilderIncorrectParametersOrder=new StringBuilder();
			//Get type definitions from OpenDentBusiness that need to have their parameters checked.
			//Do not check method signatures for CRUD and Meth types.
			List<TypeDefinition> listTypeDefinitions=GetTypeDefinitions()
				.FindAll(x => x.Name!=typeof(Meth).Name && !x.FullName.StartsWith("OpenDentBusiness.Crud"));
			List<MethodDefinition> listMethodDefinitions=GetMethodDefinitions(listTypeDefinitions:listTypeDefinitions);
			foreach(MethodDefinition method in listMethodDefinitions) {
				if(method.Name==SymbolExtensions.GetMethodInfo(() => WebServiceTests.InvalidWebMethod()).Name) {
					continue;
				}
				//The assembly level instructions.
				List<Instruction> listMethodInstructions=method.Body.Instructions.ToList();
				//Find the instruction that is the call to Meth.GetTable, Meth.GetObject, etc.
				Instruction methodCallInstructionMeth=GetInstructionCallingMeth(method);
				//Handle any methods that do not invoke a "Meth" method.
				if(methodCallInstructionMeth==null) {
					#region No DB calls Check
					//Skip any non s class methods. Not worth worrying about these classes because they are typically handled in strange fashions.
					//E.g. classes such as ConvertDatabasesX.cs will always have a MiddleTierRole check done prior to invocation.
					if(!listSClassNames.Contains(method.DeclaringType.Name.ToLower())) {
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
						strBuilderNoMiddleTierRolesCheck.AppendLine($"Class: {method.DeclaringType.Name}\r\nMethod: {method.Name}\r\n");
					}
					continue;
					#endregion 
				}
				//We know it calls a MiddleTierRole method. To tell how many parameters are passed in, we need to look for the LDC (load) commands
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
						&& method.Name==SymbolExtensions.GetMethodInfo(() => Schedules.Insert(false,false,null)).Name)
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
			if(strBuilderNoMiddleTierRolesCheck.Length!=0) {
				listErrors.Add("The following methods call the database (crud/db/datacore) without a MiddleTierRole check.\r\n"
					+strBuilderNoMiddleTierRolesCheck.ToString());
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

		///<summary>This test ensures all public static void S class methods have a return within their middle tier check.
		///For example,
		///if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
		///		Meth.GetVoid(MethodBase.GetCurrentMethod());
		///		return;
		///}
		///</summary>
		[TestMethod]
		public void MiddleTier_ReturnAfterVoid() {
			StringBuilder strBuilderMissingReturn=new StringBuilder();
			List<MethodDefinition> listMethodDefinitionsSClasses=GetMethodDefinitions();
			foreach(MethodDefinition method in listMethodDefinitionsSClasses) {
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
				//if(RemotingClient.MiddleTierRole==RemotingRole.ClientMT) { }
				//To do this, we can find the instruction where the if begins (RemotingClient.MiddleTierRole) and the instruction where the if ends. 
				//If we look at the instructions in between the two, we can see if 'ret' is called or if we branch to the ret instruction.
				Instruction getMiddleTierRoleInstruction=listMethodCallInstructions.FirstOrDefault(x => 
					((MethodDefinition)x.Operand).DeclaringType.FullName==typeof(RemotingClient).FullName
					&& ((MethodDefinition)x.Operand).Name==$"get_{nameof(RemotingClient.MiddleTierRole)}");
				if(getMiddleTierRoleInstruction==null) {
					throw new Exception("Cannot find MiddleTierRole getter instruction.");
				}
				int indexOfMiddleTierRoleInstruction=listMethodInstructions.IndexOf(getMiddleTierRoleInstruction);
				//To figure out where the if ends, we need to find the branch if false command and what instruction it leads to. We will look for 
				//the first command starting after the instruction above.
				Instruction branchIfFalseInstruction=listMethodInstructions.GetRange(indexOfMiddleTierRoleInstruction,
					listMethodInstructions.Count-indexOfMiddleTierRoleInstruction)
					.FirstOrDefault(x => x.OpCode.In(OpCodes.Brfalse,OpCodes.Brfalse_S));
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
					else if(instruction.OpCode.In(OpCodes.Br_S,OpCodes.Br)) {
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

		///<summary>This test ensures there are NO private static methods that call Meth.Get...</summary>
		[TestMethod]
		public void MiddleTier_NoPrivateMeth() {
			StringBuilder strBuilderPrivateMeth=new StringBuilder();
			//Get all private methods from the assembly.
			List<MethodDefinition> listMethodDefinitionsPrivate=GetMethodDefinitions(isPublic:false);
			foreach(MethodDefinition method in listMethodDefinitionsPrivate) {
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

		///<summary>This test ensures there is only one Meth invocation per S class method.</summary>
		[TestMethod]
		public void MiddleTier_MultipleMethInvocations() {
			StringBuilder strBuilderPrivateMeth=new StringBuilder();
			List<MethodDefinition> listMethodDefinitions=GetMethodDefinitions();
			foreach(MethodDefinition method in listMethodDefinitions) {
				int count=GetMethInvocationCount(method);
				if(count > 1) {
					strBuilderPrivateMeth.AppendLine($"Class: {method.DeclaringType.Name}\r\nMethod: {method.Name}\r\n");
				}
			}
			if(strBuilderPrivateMeth.Length!=0) {
				Console.WriteLine("The following methods have multiple calls to Meth methods. There should only be one.");
				Console.WriteLine(strBuilderPrivateMeth.ToString());
			}
			Assert.AreEqual(0,strBuilderPrivateMeth.Length);
		}

		///<summary>This test ensures all database calls have at least one MiddleTierRole check in the stack trace.
		///It finds all database calls in private static methods in our S classes, which by nature have no MiddleTierRole check.
		///Then it looks at all references to these methods and traces its way up each potential call stack until it reaches a public static method.
		///The stack trace is valid when there is a MiddleTierRole check. Otherwise, the stack trace path is flagged as invalid.
		///Other tests ensure public methods have MiddleTierRole checks in the case of database calls directly in that method.
		///Also, other tests ensure that private static methods do not have erroneous MiddleTierRole checks.</summary>
		[TestMethod]
		public void MiddleTier_PathToError() {
			StringBuilder strBuilderBadPaths=new StringBuilder();
			List<TypeDefinition> listTypeDefinitions=GetTypeDefinitionsForSClasses();
			//This is a dictionary where the key is the method definition of a method. The value is a list of methods that call the key method. 
			//Only fills static methods.
			Dictionary<MethodDefinition,List<MethodDefinition>> dictMethodNameToMethodReferences=FillDictOfMethodReferences(listTypeDefinitions);
			//Get all methods from the assembly.
			List<MethodDefinition> listMethodDefinitionsPrivate=GetMethodDefinitions(isPublic:false,listTypeDefinitions:listTypeDefinitions);
			List<MethodDefinition> listMethodDefinitionsPrivateWithDbCall=listMethodDefinitionsPrivate.FindAll(x => DoesMethodCallTheDatabase(x));
			foreach(MethodDefinition method in listMethodDefinitionsPrivateWithDbCall) {
				CheckReferencesForMethod(method,strBuilderBadPaths,new List<MethodDefinition>(),dictMethodNameToMethodReferences);
			}
			if(strBuilderBadPaths.Length!=0) {
				Console.WriteLine("The following paths can cause an error. There are no MiddleTierRole checks along the path. There is a database call "
					+"along the path, however.");
				Console.WriteLine(strBuilderBadPaths.ToString());
			}
			Assert.AreEqual(0,strBuilderBadPaths.Length);
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

		///<summary>Checks the given method to find if it is a bad path. First checks to see if the method is public static. If it is, and it does not
		///call a MiddleTierRole check, this is an error. If it is private, it will continue up the potential path via references to the method
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
					//This means the method has a MiddleTierRole check. This is a valid path. Nothing needs to be done.
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
		private static Dictionary<MethodDefinition,List<MethodDefinition>> FillDictOfMethodReferences(List<TypeDefinition> listTypeDefinition) {
			Dictionary<MethodDefinition,List<MethodDefinition>> dict=new Dictionary<MethodDefinition,List<MethodDefinition>>();
			//Find all static methods.
			List<MethodDefinition> listMethods=listTypeDefinition
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

		///<summary>Returns a list of method definitions that have a body from S classes.
		///Optionally pass in a list of type definitions to only get methods associated with those types.</summary>
		private static List<MethodDefinition> GetMethodDefinitions(bool isPublic=true,bool isStatic=true,List<TypeDefinition> listTypeDefinitions=null) {
			if(listTypeDefinitions==null) {
				listTypeDefinitions=GetTypeDefinitionsForSClasses();
			}
			List<MethodDefinition> listMethodDefinitions=listTypeDefinitions.SelectMany(x => x.Methods)
				.Where(x => x.HasBody && x.IsPublic==isPublic && x.IsStatic==isStatic)//Needs to have a body so we can look through its contents.
				.ToList();
			return listMethodDefinitions;
		}

		///<summary>Returns a list of lowercase strings that represent the names of the S classes in OpenDentBusiness.</summary>
		private static List<string> GetSClassNames() {
			List<string> arraySClassPaths=Directory.GetFiles(@"..\..\..\OpenDentBusiness\Data Interface").ToList();
			arraySClassPaths.AddRange(Directory.GetFiles(@"..\..\..\OpenDentBusiness\Db Multi Table"));
			//Get just the names of the classes without the path and extension information.
			return arraySClassPaths.Select(x => Path.GetFileNameWithoutExtension(x).ToLower()).ToList();
		}

		///<summary>Returns all type definitions from the OpenDentBusiness assembly.</summary>
		private static List<TypeDefinition> GetTypeDefinitions() {
			AssemblyDefinition assemblyDefinition=AssemblyDefinition.ReadAssembly(@"OpenDentBusiness.dll");
			List<TypeDefinition> listTypeDefinitions=assemblyDefinition.Modules
				.SelectMany(x => x.Types)
				.ToList();
			return listTypeDefinitions;
		}

		private static List<TypeDefinition> GetTypeDefinitionsForSClasses() {
			List<string> listSClassNames=GetSClassNames();
			List<TypeDefinition> listTypeDefinitions=GetTypeDefinitions();
			List<TypeDefinition> listTypeDefinitionsSClasses=listTypeDefinitions
				.FindAll(x => listSClassNames.Contains(x.Name.ToLower()));
			return listTypeDefinitionsSClasses;
		}

		///<summary>Returns the instruction from the method that is the call to any Meth.Get... method.
		///Returns null if the method does not have a body or the instruction could not be found.</summary>
		private static Instruction GetInstructionCallingMeth(MethodDefinition method) {
			if(!method.HasBody) {
				return null;
			}
			List<Instruction> listInstructions=method.Body.Instructions.ToList();
			for(int i=0;i<listInstructions.Count;i++) {
				if(listInstructions[i].Operand==null) {
					continue;
				}
				if(listInstructions[i].OpCode!=OpCodes.Call) {
					continue;
				}
				//Typical Meth methods utilze MethodDefinition. However, Meth.GetObject<T> utilizes GenericInstanceMethod. Check for both.
				//Ignore instructions that invoke Meth.NoCheckMiddleTierRole() since it is not a Get... method.
				if(listInstructions[i].Operand is MethodDefinition methodDefinition
					&& methodDefinition.DeclaringType.FullName==typeof(Meth).FullName
					&& methodDefinition.Name!=nameof(Meth.NoCheckMiddleTierRole))
				{
					return listInstructions[i];
				}
				if(listInstructions[i].Operand is GenericInstanceMethod genericInstanceMethod
					&& genericInstanceMethod.DeclaringType.FullName==typeof(Meth).FullName
					&& genericInstanceMethod.Name!=nameof(Meth.NoCheckMiddleTierRole))
				{
					return listInstructions[i];
				}
			}
			return null;
		}

		private static int GetMethInvocationCount(MethodDefinition method) {
			int count=0;
			if(!method.HasBody) {
				return count;
			}
			List<Instruction> listInstructions=method.Body.Instructions.ToList();
			for(int i=0;i<listInstructions.Count;i++) {
				if(listInstructions[i].Operand==null) {
					continue;
				}
				if(listInstructions[i].OpCode!=OpCodes.Call) {
					continue;
				}
				//Typical Meth methods utilze MethodDefinition. However, Meth.GetObject<T> utilizes GenericInstanceMethod. Check for both.
				if((listInstructions[i].Operand is MethodDefinition methodDefinition && methodDefinition.DeclaringType.FullName==typeof(Meth).FullName)
					|| (listInstructions[i].Operand is GenericInstanceMethod genericInstanceMethod && genericInstanceMethod.DeclaringType.FullName==typeof(Meth).FullName))
				{
					count++;
				}
			}
			return count;
		}

		///<summary>Returns if the passed in method makes a database call that would need to be wrapped in a middle tier check.</summary>
		private static bool DoesMethodCallTheDatabase(MethodDefinition method) {
			//Check to see if there are any method calls to the Crud or to any method in Db. These all need to be wrapped in a MiddleTierRole check.
			List<Instruction> listCallingInstructions=method.Body.Instructions.Where(x => x.Operand!=null && x.OpCode==OpCodes.Call)
				.Where(x => (x.Operand as MethodDefinition)!=null).ToList();
			bool isCrudCalled=listCallingInstructions
				.Any(x => ((MethodDefinition)x.Operand).DeclaringType.FullName.StartsWith("OpenDentBusiness.Crud") 
				&& !((MethodDefinition)x.Operand).Name.In("TableToList","ListToTable","UpdateComparison"));//Skip these as they do not call the db.
			if(isCrudCalled) {
				//Skip the db check to speed up the process if we know the method already calls the crud.
				return true;
			}
			return listCallingInstructions.Any(x => ((MethodDefinition)x.Operand).DeclaringType.Name.In(typeof(Db).Name,typeof(DataCore).Name));
		}

	}
}
