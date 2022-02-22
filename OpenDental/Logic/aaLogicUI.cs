//There is another Logic folder in OpenDentBusiness that is for certain kinds of logic that does not need access to OpenDental classes.
//The classes in this folder should not make any calls to the Db, but they can call S classes.
//They can access other OpenDental classes.
//They can have UI elements in them, such as MessageBoxes.
//They can help encapsulate or centralize processing that doesn't really involve the form.
//Once they handle MsgBoxes, etc, they can pass off their work to the business layer S classes.
