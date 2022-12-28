# techgen back
```
http://myserver.com/api/Account/Register
Body: {email, password, repeatedPassword}

http://myserver.com/api/Account/Login 
Body: {email, password}

http://myserver.com/api/Account/SendRecoveryCode?email=UserEmail&recoveryCode=RecoveryCode
Query: UserEmail - user email, RecoveryCode - (string) recovery code which sent during registration 

http://myserver.com/api/Account/ChangePassword?email=UserEmail&recoveryCode=RecoveryCode 
Query: UserEmail - user email, RecoveryCode - (string) new recovery code which sent during SendRecoveryCode

http://myserver.com/api/Account/CreateRole?name=RoleName 
Query: RoleName - (string) name of role 
```
