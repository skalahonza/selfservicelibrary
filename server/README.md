# Core
Jadro aplikace obsahuje nezbytný kód k fungování samoobslužné knihvny. Rozumí systémovým entitám (kniha, výpůjčka, uživatel) a implementuje funkční požadavky (půjčit knihu, vrátit knihu). Ke svému fungování často potřebuje i další závislosti např.: mapování na DTO a zpět, email, CSV import a další. Nemělo by však záležet na konkrétní implementaci. Jádro **core** definuje jaké závislosti potřebuje pomocí rozhraní (interface). Tato rozhraní pak konzumují další vrstvy a implementují je. Když se pak později změní implementace konkrétní závislosti (například přestaneme používát ČVUT smtp server a místo toho použijeme SendGrid k odesílání emailů) kód aplikačního jádra nebude vůbec nutné měnit. Zároveň se kód stává testovatelným, protože každé rozhraní můžeme v testech [Mockovat](https://en.wikipedia.org/wiki/Mock_object).

## SelfServiceLibrary.Domain
Obsahuje doménový model aplikace: výčtové typy, případně vlastní datové typy. Nemá žádné externí závislosti.

## SelfServiceLibrary.Service
Závisí na **Domain**. Poskytuje služby, které dokáží se systémovými entitami zacházet. Příklad: Máme entitu **Book** a službu **BookService**. Služba **BookService** dokáže entitu **Book** najít, zobrazit seznam, vytvořit, uložit nebo upravit. Tuto vrstvu konzumují: webová aplikace a webová služba API. Tato a jen tato vrstva implementuje funkční požadavky.

# Infrastructure
Aby jádro aplikace **core** potřebuje ke správnému fungování několik externích závislostí. Například CSV parser, mapování z DTO na databázovou entitu a zpět. Konkrétní implementace by však neměla zásadním způsobem ovlivnit fungování aplikace (když na pozadí vyměníme knihovnu na parsování CSV nemělo by být zapotřebí měnit kod jádra) a proto je vhodné držet kód infrastruktury mimo.

## SelfServiceLibrary.Persistence
Tato vrstva přímo komunikuje s databází. Databáze často umí jen operace CRUD (create, read, update, delete). Relační databáze umí navíc i transakce. Převádí operace servisní vrstvy (půjčit si knihu) na operace pro databázový stroj (najdi knihu podle id, zjisti zda je k dispozici, zamkni ji, založ výpůjčku, při chybě obnov stav).

## SelfServiceLibrary.CSV
Implementuje rozhraní **ICsvImporter**, které poskytuje **core** pomocí knihovny [CsvHelper](https://joshclose.github.io/CsvHelper/)

## SelfServiceLibrary.Mapping
Implementuje **IMapper** rozhraní, které poskytuje **core** pomocí frameworku [AutoMapper](https://automapper.org/). Stará se o mapování mezi objekty. Například mapování Book --> BookListDTO.

# Presentation
Obsahuje prezentační vrstvu aplikace. Patří sem webová aplikace, API služba, případně další aplikace s UI.

## SelfServiceLibrary Web
Server Side Blazor webová aplikace pro návštěvníky knihovny a knihovníky.

## SelfServiceLibrary API
REST API služba. Slouží ke komunikaci mezi databází a Kiosk aplikací. Autentizuje uživatele pomocí ID karty, případně PIN kódu. 

## Kiosk
Aplikace běžící na terminálu v knihovně. Disponuje čtečkou karet a ovládáacím rozhraním. Nekomunikuje s databází, ale pouze s API službou prostřednictví REST a HTTP.

# Tests
Obsahuje kod pro testování aplikace. Zde se nepřidává žádná nová funkcionalita.