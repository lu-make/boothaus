<img width="300" height="150" alt="Boothaus" src="https://github.com/user-attachments/assets/de3fbb4b-9b5e-4129-b38a-9aa1cc346139" />
  
Eine einfache Lagerverwaltungssoftware speziell für Winterlager für Sport- und Freizeitboote

## Funktionsumfang

Die Software erlaubt die Verwaltung von Booten sowie die Verteilung von Lageraufträgen und Lagerplätzen pro Wintersaison. Es gibt eine zweidimensionale Übersicht über Lagerplätze als Raster und die Möglichkeit, Lageraufträge automatisch auf freie Lagerplätze zu verteilen. Sie können außerdem Daten exportieren und importieren.

## Anforderungen 

Das Programm läuft unter Windows in x86, x64 und ARM64. Es muss eine .NET Runtime installiert sein.

## Gebrauch

Laden Sie die entsprechende .ZIP Datei aus "Releases" herunter und extrahieren Sie sie, darin finden Sie eine ausführbare Datei namens "Boothaus.exe".

<img width="900" height="auto" alt="Anwendung" src="https://github.com/user-attachments/assets/0fb9f563-ae59-47c3-8cbe-1ac7a70e1904" />

Links befindet sich eine Liste von Aufträgen. Oben können Sie im Menü die aktuelle Saison wählen. Es gibt grundsätzlich einen Auftrag für ein Boot pro Saison. Aufträge können automatisch Plätzen zugewiesen werden. Die Platzverteilung funktioniert immer so, dass Aufträge die in einer Reihe sind, zeitlich miteinander verschachtelt sind. Das heißt dass der hintere Auftrag (weiter weg vom Eingang) einen früheren Einwinterungstermin und einen späteren Auswinterungstermin hat, als der vordere Eintrag (näher am Eingang).

Rechts sehen Sie eine zweidimensionale Übersicht der Lagerplätze. Es werden freie und belegte Plätze angezeigt. Sie können Lageraufträge ziehen und auf einen freien Lagerplatz ablegen. Es wird automatisch angezeigt, welche Lagerplätze verfügbar sind. Sie können Aufträge auch zwischen Plätzen hin und her ziehen, oder die Platzzuweisung löschen indem sie von einem Platz nach außerhalb ziehen. 

Sie können mit den +/- Buttons Lagerplätze und Lagerreihen hinzufügen und entfernen. Die maximale Länge und Breite von Booten kann über die Einstellungen angepasst werden.

Um alle Boote zu sehen und zu verwalten, klicken Sie auf "Boote verwalten".

<img width="400px" height="auto" alt="Boote verwalten" src="https://github.com/user-attachments/assets/01ae1b2c-7543-4ace-8c7c-13129aab38b2" />

Aufträge können neu erfasst oder bearbeitet werden, dann öffnet sich eine Maske in welcher Sie ein Boot, ein Startdatum und ein Enddatum auswählen können.

<img width="300px" height="auto" alt="Auftrag erfassen" src="https://github.com/user-attachments/assets/8e7d210a-dbe2-4f91-a323-b441cf0c7b79" />

Wenn Sie ein neues Boot anlegen möchten können Sie entweder hier "Neues Boot" oder in der Bootverwaltung auf "Erfassen" klicken.

<img width="300px" height="auto" alt="Boot erfassen" src="https://github.com/user-attachments/assets/24c86684-c35b-49d6-b3e0-56d28aa40b7b" />

Das Lager hat eine maximale Länge und Breite pro Boot. Das können Sie unter "Bearbeiten"->"Einstellungen" ändern.

<img width="300px" height="auto" alt="Einstellungen" src="https://github.com/user-attachments/assets/d40f2109-f923-46b7-a5b6-87b070494587" />

