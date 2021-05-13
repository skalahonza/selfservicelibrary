# Self Service Library
Tato práce se zabývá tvorbou informačního systému umožňujícího správu inventáře knihovny na vybrané katedře ČVUT. Kromě základních funkcí bude poskytovat i možnost samoobslužného provozu. Výstupem práce jsou celkem 4 aplikace: Webová aplikace zobrazuje data a umožňuje přihlášení pomocí uživatelského jména a hesla ČVUT. Desktopová aplikace na kiosku zobrazující webovou aplikaci ve vestavěném prohlížeči poskytuje integraci s připojeným hardwarem. REST API služba komunikuje s kioskem. Služba na pozadí připomíná uživatelům datum vrácení zapůjčené publikace.

This work’s goal is to create an information system allowing management of libraries provided by university departments. Besides core functionality the system also provides self-service features. The output of this thesis are 4 applications: A web application displaying data and authenticating users using existing CTU credentials. Desktop application (kiosk) showing the web app in an embedded browser and integrating with connected hardware. REST API service communicating with kiosk. And finally, a background service reminding users to return publications they borrowed. 

# Aplikace
## Webová aplikace (SelfServiceLibrary Web)
* .NET 5
* Server side blazor

Hlavní část představující webové rozhraní. Zpracovává vstupy uživatele a poskytuje požadovaná data. Zobrazuje se na zařízeních uživatele (počítač, telefon, tablet), případně na kiosku v knihovně.

## REST API služba (SelfServiceLibrary API)
* .NET 5
* Web API
Umožňuje kiosku vyhledávat knihy podle NFC štítků a autentizovat uživatele pomocí přístupové NFC karty.

## Služba na pozadí (SelfServiceLibrary BG)
* .NET 5
* Background worker
Upozorňuje uživatele na vrácení knihy.

## Desktopová aplikace na kiosku
* .NET 5
* WPF
Není součástí tohoto repozitáře. Zobrazuje webovou aplikaci (A) ve webovém prohlížeči. Komunikuje s připojenou NFC čtečkou a kamerou pro skenování QR kódů.

# Local development
* Visual Studio
* Docker

## Registering an app 
1. Go to https://auth.fit.cvut.cz/manager/index.xhtml
2. Create a new project
3. Click on Services
4. Enable scopes for Usermap API
    * urn:ctu:oauth:umapi.read
    * cvut:umapi:read
5. Create application (type: Web Application)
6. Set Redirect URI to `https://localhost:44302/`
7. Get your Client ID and Client Secret
8. Right click on **Manage User Secrets** on SelfServiceLibrary.Web project and make it look like this:
```json
{
  "SendGrid:ApiKey": "your send grid api key",
  "oAuth2:RedirectUri": "https://localhost:44302/sign-in",
  "oAuth2:ClientSecret": "Client Secret",
  "oAuth2:ClientId": "Client ID",
  "kiosk:SecretKey": "kiosk secret key, the same that is in KIOSK app config",
  "Admins:0": "your cvut username e.g. skalaja7"
}
```
9. Create application (type: Service Account)
10. Get your Client ID and Client Secret
11. Modify previous json and add:
```json
{
  "usermap:ClientSecret": "Client Secret",
  "usermap:ClientId": "Client ID",
}
```
12. Make Docker Compose your startup project
13. Press F5
