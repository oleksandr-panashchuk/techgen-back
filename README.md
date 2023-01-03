# techgen back
```
https://techgenapp.azurewebsites.net/api/Account/Register
Body: {email, password, repeatedPassword}

https://techgenapp.azurewebsites.net/api/Account/Login 
Body: {email, password}

https://techgenapp.azurewebsites.net/api/Account/SendRecoveryCode?email=UserEmail&recoveryCode=RecoveryCode
Query: UserEmail - user email, RecoveryCode - (string) recovery code which sent during registration 

https://techgenapp.azurewebsites.net/api/Account/ChangePassword?email=UserEmail&recoveryCode=RecoveryCode 
Query: UserEmail - user email, RecoveryCode - (string) new recovery code which sent during SendRecoveryCode

https://techgenapp.azurewebsites.net/api/Account/CreateRole?name=RoleName 
Query: RoleName - (string) name of role 
```
