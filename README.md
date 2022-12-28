# techgen back
```
http://myserver.com/Account/Register
Body: {"Email": "username@gmail.com", "Password": "GG12_3fgaaf", "PasswordConfirm": "GG123_fgaaf"}

http://myserver.com/Account/Login 
Body: {"Email": "username@gmail.com", "Password": "GG12_3fgaaf",}

http://myserver.com/Account/SendRecoveryCode 
Query: http://myserver.com/Account/SendRecoveryCode?email=username@gmail.com&RecoveryCode=143545

http://myserver.com/Account/ChangePassword 
Query: https://localhost:44322/api/Account/ChangePassword?email=username@gmail.com&RecoveryCode=143545

http://myserver.com/Account/CreateRole 
Query: https://localhost:44322/api/Account/CreateRole?name=RoleName
```
