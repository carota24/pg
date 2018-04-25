<b>This is a first example of REST API Azure Function.</b>
I did not use output binding to cosmos because async functions do not support output binding.
The only way i found it was basically to use cosmos directly in c# , I keep the connection string as part of application.settings variables
