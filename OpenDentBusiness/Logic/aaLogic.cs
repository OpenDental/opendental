//These notes describe when to use this Logic folder in OpenDentBusiness.

//There is another Logic folder in OpenDental that is slightly different.
//Use that one when there are MsgBoxes or you don't need to access from OpenDentBusiness.

//Classes in this Logic folder typically end in "Logic" rather than s.  
//They contain business logic that does not belong in the UI.  
//They are usually not associated with any particular table in the database, or else they would probably just go in the appropriate S class.
//These methods should not contain queries.
//For queries, use an S class in the Data Inteface folder.
//Or use Db Multi Table if there are many db tables involved, so it's hard to pick an S class.
 
//Classes in this folder can use local Cache without any problems.
  