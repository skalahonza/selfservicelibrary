# Self Service Library
Tato práce se zabývá tvorbou informačního systému umožňujícího správu inventáře knihovny na vybrané katedře ČVUT. Kromě základních funkcí bude poskytovat i možnost samoobslužného provozu. Výstupem práce jsou celkem 4 aplikace: Webová aplikace zobrazuje data a umožňuje přihlášení pomocí uživatelského jména a hesla ČVUT. Desktopová aplikace na kiosku zobrazující webovou aplikaci ve vestavěném prohlížeči poskytuje integraci s připojeným hardwarem. REST API služba komunikuje s kioskem. Služba na pozadí připomíná uživatelům datum vrácení zapůjčené publikace.

This work’s goal is to create an information system allowing management of libraries provided by university departments. Besides core functionality the system also provides self-service features. The output of this thesis are 4 applications: A web application displaying data and authenticating users using existing CTU credentials. Desktop application (kiosk) showing the web app in an embedded browser and integrating with connected hardware. REST API service communicating with kiosk. And finally, a background service reminding users to return publications they borrowed. 

# Struktura projektu
## Složka data
Obsahuje exportovaná CSV data z původní katederní knihovny. Při vývoji je lze nahrát jako testovací data na stránce Settings po stisknutí tlačítka **Import CSV**. Zároveň obsahuje i skript build.ps1, který všechny CSV soubory sjednotí a vygeneruje soubor pro program Microsoft Excel. Lze tak data data hromadně prohlížet nebo editovat.

## Složka hosting
Obsahuje Docker Compose konfigurační soubor pro produkční prostředí. Na portu 80 se spustí webová reverzní proxy, která všechny požadavky začínající na "/api" přesměruje na REST API službu a zbytek do webové aplikace. 

## Složka scripts
Obsahuje pomocné skripty v jazyce PowerShell:
* countries.ps1 - zobrazí všechny země vydání ze všech CSV souborů
* types.ps1 - zobrazí všechny typy publikací ze všech CSV souborů
* Update-Csv.ps1 - stáhne exportovaná CSV data z původní starší katederní knihovny

Skripty slouží pouze k usnadnění práce během vývoje a k chodu aplikace nejsou potřeba.

## Složka server
Složka obsahuje veškerý zdrojový kód, který je spuštěn na straně serveru katederní knihovny.

# Aplikace
## Webová aplikace (SelfServiceLibrary Web)
* .NET 5
* Server Side Blazor

Hlavní část představující webové rozhraní. Zpracovává vstupy uživatele a poskytuje požadovaná data. Zobrazuje se na zařízeních uživatele (počítač, telefon, tablet), případně na kiosku v knihovně.

## REST API služba (SelfServiceLibrary API)
* .NET 5
* Web API
Umožňuje kiosku vyhledávat knihy podle NFC štítků a autentizovat uživatele pomocí přístupové NFC karty.

## Služba na pozadí (SelfServiceLibrary BG)
* .NET 5
* Background worker 
* Upozorňuje uživatele na vrácení knihy.

## Desktopová aplikace na kiosku
* .NET 5
* WPF
Není součástí tohoto repositáře. Zobrazuje webovou aplikaci (A) ve webovém prohlížeči. Komunikuje s připojenou NFC čtečkou a kamerou pro skenování QR kódů.

# Konfigurace
Veškerá konfigurace se provádí pomocí [Dotnet User Secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-5.0&tabs=linux).

## Webová aplikace (SelfServiceLibrary Web)
| Název                | Význam                                                  |
|----------------------|---------------------------------------------------------|
| usermap:ClientId     | Client ID do USERMAP                                    |
| usermap:ClientSecret | Client Secret do USERMAP                                |
| SendGrid:ApiKey      | API klíč pro službu SnedGrid                            |
| oAuth2:RedirectUri   | Url pro přesměrování připoužívání OAuth 2.0             |
| oAuth2:ClientId      | Client ID pro Zuul                                      |
| oAuth2:ClientSecret  | Client Secret pro Zuul                                  |
| kiosk:SecretKey      | Tajný klíč pro OTP ověření na kiosku                    |
| Admins:0             | ČVUT login hlavního admina knihovny (příklad: skalaja7) |
## REST API služba (SelfServiceLibrary API)
Volitelně lze nastavit API klíče, které budou vyžadovány při každém požadavku. Pokud API klíč není nastaven nebo je v konfiguraci uveden prázdný, tak aplikace nevyžaduje autentizaci.

| Název                | Význam                                                  |
|----------------------|---------------------------------------------------------|
| ApiKeys:0              | Api Klíč shodný s API klíčem nastaveným na kiosku     |
## Služba na pozadí (SelfServiceLibrary BG)
| Název                | Význam                                                  |
|----------------------|---------------------------------------------------------|
| SendGrid:ApiKey      | API klíč pro službu SnedGrid                            |

# Lokální vývoj
* Visual Studio
* Docker

## Registering an app 
1. Jděte na https://auth.fit.cvut.cz/manager/index.xhtml
2. Vytvořte nový projekt
3. Klikněte na Services
4. Povolte oprávnění Usermap API
    * urn:ctu:oauth:umapi.read
    * cvut:umapi:read
5. Vytvořte aplikaci (type: Web Application)
6. Nastavte Redirect URI to `https://localhost:44302/`
7. Okopírujte si Client ID a Client Secret
8. Pravým tlačítkem klikněte na **Manage User Secrets** u SelfServiceLibrary.Web projektu a vyplňte následující údaje:
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
9. Vytvořte aplikaci (type: Service Account)
10. Okopírujte siClient ID and Client Secret
11. Upravte předchozí json a přidejte:
```json
{
  "usermap:ClientSecret": "Client Secret",
  "usermap:ClientId": "Client ID",
}
```
12. Označte Docker Compose projekt jako startup project
13. Stiskněte F5
