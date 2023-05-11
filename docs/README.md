<div align="center">
  
  <h1> Device Base </h1>

<br/>
  
</div>



### Opis Systemu

System jest globalny oparty o SQL Server.
System służy do przechowywania informacji o urządzeniach typu kontroler pasków led / inteligentna żarówka / inteligentna brama.

Każde fizyczne urządzenie ma swój typ, typ zawiera podstawowe informacje wspólne dla wszystkich urządzeń np maksymalna liczba użytkowników która może się podłączyć do urządzenia. Typ jest tak jak model urządzenia, w bazie nie przechowywujemy wszytkich informacji o modelu urzadzenia raczej zakładamy że tutaj administrator może wprowadzic nowy model z innego systemu np. stricte zwiazanego już z zespołem tworzącym nowe modele urządzeń.

System zakłada że informacje o fizycznym urządzeniu są zawsze w bazie danych tzn. są dodawane przy wypuszczaniu urzadzenia z fabryki i dodawane do bazy danych przez administratora. Następnie użytkownik może się do nich połaczyć i edytować wybrane wartośći urządzenia. (Proces podłączania do bazy opisany niżej)

System zawiera 2 głównych aktorów:
- Zautoryzowanego użytkownika - posiada ograniczone uprawnienia w systemie - nie może wysyłać zapytań na niektóre endpointy np nie może dodawać urządzeń
- Administratora - może wysyłać zapytanie na każdy endpoint - administrator dba miedzy innymi o dodawnie nowych typów urządzen do bazy oraz fizycznych urządzeń

Endpointy możemy pogrupować:
- Endpointy Autoryzacji - ma na celu logowanie / rejestrowanie użytkownika bądź odświerzania tokenów użytkownika (autoryzacjia oparta o JWT i OAuth 2)
- Endpointy Urządzeń - zawiera podstawowe operacji CRUD oraz metody pozwalające na łączenie się z urządzeniami metody te są wywoływane przez aplikacje po połączeniu sie użytkownika z urządzeniem
- Endpointy Typów Urządzeń - zawiera podstawowe operacje CRUD dla modeli urządzeń
- Endpointy Użytkownika - Pozwala użytkownikowi na modyfikowanie niekórych swoich ustawień oraz pobieranie ich 

#### Baza danych 

<div> 
    <img src="https://github.com/psp515/DeviceBase/blob/main/docs/database.png" />
</div>


Baza danych zawiera wiele tabel, wiekszość z nich jest generowana na potrzeby wewnętrze Identity Component który wykorzystywany jest do autoryzacji i autentykacji.
Oprócz tego baza zawiera tabele z urządzeniami oraz ich typami oraz tabele przejścia pomiedzy urządzeniami oraz uytkownikami.

#### Przykładowy przypadek użycia - Łaczenie sie do urządzenia przez właściciela (który chwile temu kupił produkt)

1. Z telefonem z aplikacją jesteśmy w zasiegu bluetooth urządenia.
2. Aplikacja wykrywa urządzenie i propouje połaczenie sie z nim jako gość / jako jedyny właściciel.
3. Użytkownik wybiera opcje 2.
4. Aplikacja pyta o sekretny klucz urządzenia (klucz dostarczony z urządzeniem w pudełku / badz hasło gdzies na urzadzeniu jak np. z routerami jest).
5. Użytkownik podaje hasło i oczekuje na połączenie.
6a. Udane połączenie można korzystać z urządzenia.
6b. Nieudane połączenie urządzenie posiada włąsciciela - jezeli kupilismy nowy produkt to powinnismy sie skontakowac z supportem.

#### Przykładowy przypadek uzycia 2 - Łaczenie sie do urządzenia przez kolege

1. Z telefonem z aplikacją jesteśmy w zasiegu bluetooth urządenia.
2. Aplikacja wykrywa urządzenie i propouje połaczenie sie z nim jako gość / jako jedyny właściciel.
3. Użytkownik wybiera opcje 1.
4. Aplikacja łączny nas z urządzeniem (nie pyta wlascicela o zgode).
5a. Udane połączenie można korzystać z urządzenia.
5b. Właściciel urządzenia zablokował możliwość łącznia do urządzenia.

#### Przykładowy przypadek uzycia 3 - Rozłączenie właścicela

Przydatne gdy sprzedajemy urzadzenie.

1. W aplikacji klikamy rozłącz z urządzeniem.
2. Aplikacja rozłącza włascicela oraz rozłącza wszystkich urzytkowników poineważ urzadzenie nie ma wałściciel.
