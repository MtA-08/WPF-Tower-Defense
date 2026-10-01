# WPF Tower Defense

Ein Tower-Defense-Spiel für Windows, entwickelt mit C# und WPF. Gegner laufen über einen festen Weg. Du platzierst Türme daneben, verbesserst ihre Angriffe und versuchst, alle **40 Wellen** zu überstehen.

Erreicht ein Gegner das Ende des Weges, verlierst du ein Leben. Besiegte Gegner bringen Geld für weitere Türme und Verbesserungen. Neben dem Einzelspielermodus gibt es einen Mehrspielermodus mit Host und Client. Dieser ist im aktuellen Quellcode für zwei Instanzen auf demselben Rechner eingerichtet.

[Spiel herunterladen](https://github.com/MtA-08/WPF-Tower-Defense/releases/latest)

## Funktionen

- **40 festgelegte Gegnerwellen:** Verschiedene Gegnergruppen, wechselnde Abstände zwischen Gegnern und mehrere Bosswellen.
- **Vier spielbare Türme:** Caveman, Archer, Gunman und Sniper mit unterschiedlichen Preisen, Reichweiten und Projektilen.
- **Fünf Gegnertypen:** Unterschiede bei Geschwindigkeit, Lebenspunkten und Geldbelohnung.
- **Unterschiedliche Projektile:** Steine, Pfeile und Kugeln mit eigenem Schaden, eigener Geschwindigkeit und unterschiedlicher Anzahl möglicher Treffer.
- **Türme verbessern:** Schaden und Angriffsgeschwindigkeit jeweils in drei Stufen erhöhen.
- **Pause und Menü:** Das Einstellungsmenü hält das Spiel an und bietet Fortsetzen, Neustart und Rückkehr zum Hauptmenü.
- **Spielstand anzeigen:** Geld, verbleibende Leben und Wellenfortschritt sind sichtbar. Für Sieg und Niederlage gibt es eigene Anzeigen.
- **LAN-Multiplayer** Funktioniert zurzeit nur auf dem gleichen Rechner.

## Herunterladen und starten

1. Im [Release-Bereich](https://github.com/MtA-08/WPF-Tower-Defense/releases/latest) die Datei **`WPFTowerDefense.zip`** herunterladen.
2. Die gesamte ZIP-Datei entpacken.
3. **`Tower Defense.exe`** im entpackten Ordner starten.

Benötigt werden Windows und eine kompatible Installation von **.NET Framework 4.8**.

### Auf demselben Rechner ausprobieren

1. Die Anwendung zweimal starten.
2. In der ersten Instanz **Multiplayer → Host Game** wählen.
3. In der zweiten Instanz **Multiplayer → Join Game** wählen.
4. Sobald die Verbindung steht, können beide Instanzen am gemeinsamen Spielfeld arbeiten.

Der Host lauscht aktuell auf der lokalen Adresse `127.0.0.1`, und der Client verbindet sich fest mit dieser Adresse. Verwendet wird TCP-Port `50000`. Eine Verbindung zwischen zwei verschiedenen Rechnern ist mit dieser Einstellung noch nicht möglich; dafür müssten die Adressen im Quellcode angepasst werden.

**Der Mehrspielermodus ist noch nicht vollständig:** Der Client kann Türme bauen und verbessern, aber der Turmverkauf ist dort noch nicht als Netzwerkaktion umgesetzt. Auch Neustart und Sieg-/Niederlagezustand werden noch nicht vollständig zwischen Host und Client übertragen.

## Hinweise zum aktuellen Stand

- Oben im Spielfeld sind noch Testschaltflächen sichtbar. Damit lassen sich einzelne Gegner erzeugen, viel Geld vergeben und ein Sieg auslösen. **Lose Game** öffnet derzeit das Turmmenü und löst keine Niederlage aus.
- Die Ausgabenanzeige am Rundenende ist vorbereitet; der zugehörige Geldzähler wird bei Käufen und Verbesserungen derzeit noch nicht erhöht.

## Technik und Projektaufbau

- **C# und WPF mit .NET Framework 4.8**.
- XAML für Fenster und Bedienelemente.
- `DrawingVisual` für Türme, Gegner und Projektile.
- SharpVectors zum Laden der SVG-Grafiken.
- Eigene Spielschleife mit festen Logikschritten von 1/120 Sekunde bei normaler Spielgeschwindigkeit.
- TCP und Newtonsoft.Json für die Kommunikation zwischen Host und Client.

## Bilder und generative KI

**Für die Erstellung der Bilder dieses Spiels wurde generative KI verwendet.**
