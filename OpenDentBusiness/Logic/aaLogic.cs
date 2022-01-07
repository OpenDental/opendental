//Classes in this Logic folder typically end in "Logic" rather than s.  
//They contain business logic that does not belong in the UI.  
//They cannot contain any queries.  
//If the logic needs access to the database, then it must be in the DataInterface folder (s class) instead.  
//Classes in this folder can use local Cache without any problems.
  
//There is also another Logic folder in the Open Dental UI layer.
//The other folder is for logic that is closer to the UI.
//Those logic classes can contain UI elements like MessageBoxes.