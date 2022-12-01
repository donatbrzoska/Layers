using UnityEngine;

public class Notes : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

/* TODO
 * 
 * ## Eingabeparameter
 * Pose der Rakel
 * -> Position
 * -> Neigung
 * -> Drehung
 * 
 * # Auswirkungen der Position
 * - Pfad der Bewegung
 * - Abstand zur Oberfläche
 *   - Druck
 *     - Druck fängt ab bestimmter Nähe zur Leinwand an und wird dann immer größer
 *     - müsste theoretisch auch beeinflussen wie groß die Auswirkung des Neigungswinkels
 *       auf die Farbmitnahme ist
 * 
 * # Auswirkungen des Neigungswinkels:
 * - kleiner
 *   -> breitere Kontaktfläche (Realisierung über Abstandsmessung)
 * - größer
 *   -> schmalere Kontaktfläche
 *   -> mehr Farbmitnahme
 * - Auswirkungen der Position
 * 
 * 
 * 
 * ## Bidirektionaler Farbaustausch
 * 
 * # Farbübertragung bei hinreichend kleinem Abstand
 * - Abstand zur Rakel
 *   - Abstand zur Farbe wäre realistischer aber könnte schwierig werden,
 *     weil vermutlich an manchen Stellen mehr Farbe als realistisch gesammelt wird
 * 
 * # Farbmitnahme bei großem Neigungswinkel und genügend Druck
 * 
 * 
 * 
 * 
 * 
 * - Bestehende Software zum Malen mit Ölfarbe anschauen
 * - Zusammenhang zwischen Neigungswinkel und Menge der Farbe untersuchen
 * - NormalMap über Sobel-Filter
 *   - Edge Cases müssen noch gemacht werden
 *   - Tests
 *   - Wert für z und scale finden
 * - Clean Rakel Button
 * - Volumen Implementierung für OilPaintSurface <--> Farbschichten Implementierung <--> Farbschichten + Volumen
 *   - es kommt sonst vor, dass Pickup alles mitnimmt, was sehr unnatürlich aussieht
 *   - absolute vs relative Volumenwerte
 * - TestOilPaintSurface_RakelView noch in dieser Form benötigt?
 * - Winklige Rakel:
 *   - Anteiliges Emit aus dem Reservoir implementieren
 *     -> Multithreading könnte hier zum Problem werden, weil nicht geklärt ist,
 *        welcher Thread die Farbe aus den Nachbarpixeln zuerst bekommt
 *     -> Parallel For genauer untersuchen
 *     -> Sieht aus wie RangedPartitioning https://devblogs.microsoft.com/pfxteam/partitioning-in-plinq/
 *   - irgendwie geht die Farbe beim Pickup verloren
 *     - nein, sie wird nur nicht wieder abgegeben, weil das mit der zurück-Rotation dann genau nicht hinhaut
 *     -> anteiliges Emit löst dieses Problem
 * - Volumen für Emit und Pickup steuerbar machen, aktuell wird sonst von der Farbmischung im Reservoir kein Gebrauch gemacht, denn dort kann nie mehr als 1 Stück Farbe liegen
 * - Canvas Snapshot Buffer (oder Delay in AddPaint auf OilPaintSurface)
 * - Irgendwas überlegen, damit sich die Farbe auch auf dem Reservoir verschiebt?
 * - Evtl. in OilPaintSurface gar nicht überprüfen, ob die Rakel über dem Canvas ist
 *   - Problem ist dadurch aktuell, dass man nicht mit der äußersten Kante der Rakel zeichnen kann, weil der Cursor schon den Canvas verlassen hat
 * - GUI: Rotation für gegebene Strichlänge ermöglichen (Winkel_Anfang, Winkel_Ende, Strichlänge)
 * 
 * GPU-Beschleunigung
 * - Pipeline anpassen
 *   - z.B.
 *      - erstmal komplett Farbe kopieren und dann schauen, wie viel noch übrig ist
 *      - wenn mehr genommen wurde, als da war, die Differenz wieder entfernen
 * - Volumenwerte als Farbwerte kodieren für billineare Interpolation
 * 
 * Farbschichten
 * - Teil wird in darunterliegende Schicht gemischt
 * - Teil wird obendraufgelegt
 *
 * PerlinNoise
 * - Fill
 * - PartialFill
 *
 * Transferrate variieren
 * -> exponentiell, linear, …
 * -> Ersatz für Biegung
 *
 * Farbmitnahme auch abhängig von Bergen machen (dicke Farbhaufen werden dann auch mitgenommen, wenn man eigentlich nur Farbe auftragen möchte)
 *
 * Farbmitnahme auch abhängig von Trockenheit
 *
 * Farbtrocknung muss je nach Farbe variierbar sein
 *
 * Menge der aufgetragenen Farbe von Platz auf Leinwand abhängig machen (Berge werden nicht noch mehr Farbe bekommen)
 *
 * Menge der aufgetragenen Farbe von Anpressdruck abhängig machen
 * -> Nähe der Rakel zur Leinwand
 * 
 * Subtraktive Farbmischung
 * 
 * Features
 * - Anpressdruck beim über die Leinwand ziehen
 * - Farbe ausfaden lassen wenn nur noch wenig Volume
 * - mehrere verschiedene Farbschichten auf der Rakel auftragen können
 *
 * Refinements
 * - Antialiasing für Rakelabdruck?
 * - Mischung mit Hintergrundfarbe des Canvas?
 * 
 * GUI
 * - Preview für Rakel-Ausrichtung und Größe
 * - Colorpicker (https://www.youtube.com/watch?v=Ng3P_1nc8YE)
 * - Clear-Rakel Paint Button
 * - Clear-Canvas Button
 * 
 * Generell was zu den Modi überlegen
 * - Nur übers Bild ziehen
 * - Farbe auf den Rakel auftragen
 *   + Menge
 *   
 * Rakel-Neigung
 * - aktuell ist die Neigung im Prinzip 0°, die Rakel liegt also immer flach auf der Leiwand auf
 * - Maske muss dann entsprechend von RakelPosition wegverschoben werden
 * 
 * Haken am Rand beim Ziehen
 * - Berechnungen optimieren -> bringt nicht wirklich was, wahrscheinlich wird Update() einfach gar nicht oft genug aufgerufen
 * - Implementierung so anpassen, dass der Rakel "Pixel für Pixel" übers Bild gezogen wird
 *   - wird Anwendung weniger flüssig machen
 * 
 * Bugs
 * - RakelWidth 0 macht trotzdem eine Linie
 * - Wertebereiche bei InputFields nicht definierbar
 *
 * Code Quality:
 * - OilPaintEngine aufräumen
 * - OilPaintSurface Referenz aus Rakel entfernen und stattdessen übergeben
 * - RakelReferenz aus OilPaintEngine entfernen
 * - RakelNormal -> Angle
 * - Tests für OilPaintSurface IsInBounds -> reicht aktuell nur weiter an FastTexture2D
 * - Tests für Rakel im initial state
     - TODO wrong usage (Apply before set values)
 * - Tests für Mocks
 * - Tests für Apply-Calls
 * - Tests für Masks mit z.B. 70° Rotation
 * - Tests für MaskApplicator CoordinateMapping:
 *   - TODO unlucky cases
 */

/* NOTIZEN
 *
 * Streifenbug
 * - Farbe wird aufgetragen (aber nur eine Schicht)
 * - beim nächsten Schritt alles außer wieder mitgenommen außer die letzte Position (das ist dann ein Streifen)
 * - beim nächsten Schritt wieder nur eine Schicht aufgetragen
 * - usw.
 * -> eher Rendering Problem
 *
 * NormalMap über Sobel-Filter
 * - schräges Ziehen macht Stufen -> das passiert, weil man für einen Pixel nur nach oben zieht -> da liegt dann mehr Farbe
 * - evtl. noch sobel_x und sobel_y invertieren?
 *   -> komischerweise nur sobel_y
 *
 */



/* Ideen
 * 
 * - Farbreservoir
 *   - Als Stack (mehrfarbig)
 *   - Als Array mit Volumenwerten (einfarbig)
 * - Menge der übertragenen Farbe
 *   - abhängig von Volumen
 *   - abhängig von Abdrucksverzerrung
 * 
 */



/* Koordinatensysteme 
 * 
 * Array-Koordinatensystem: Origin oben links und x,y vertauscht!!! (wie Rotation um 90° nach rechts)
 * Texture2D-Koordinatensystem: Origin unten links (weil 180° rotiert)
 * Screen-Koordinatensystem: Origin unten links
 * 
 * Transformationen
 * Screen -> 
 */



/* Visual Studio Tastenkombinationen
 * 
 * Q: Nach links und rechts bewegen
 * ALT: Rotieren
 *
 * Command + D/Linksklick: Springe zu Deklaration
 * CMD + CTRL + LEFT/RIGHT: Springe zu vorheriger/nächster Curserposition
 * 
 * ALT + Mouse: Spaltenweise Mutliline-Markierung
 * CTRL + ALT + Mouse: Multicurser
 */



/* Bugsources
 * 
 * 2 innere For-Schleifen
 * - die zweite modifiziert die Zählvariable der äußeren For-Schleife (i statt k) ...
 * 
 * Das ausgegebene Array ist gar nicht das Ergebnis-Array im Test, sondern ein Zwischenstand aber noch VORM Rechteck füllen.
 * Der tatsächliche Fehler war, dass ich das erwartete Ergebnis falsch definiert hatte (Maske um zwei Spalten nach links verschoben, durch vorheriges Copy-Paste)
 *
 * Vector3 kann man nicht mit == vergleichen ... (auch wenn es nicht nullable ist)
 * 
 * Copy and Paste
 * - Controller benutzt falschen Setter und tut damit natürlich was anderes als erwartet
 * 
 * Copy and Paste
 * - TestCase mit Bug kopiert
 * - Bug nur in einem TestCase gefixt
 * - Aber immer nur den anderen TestCase mit "Run Selected" ausgeführt und richtig hart gewundert ...
 * 
 * Falsche API vermutet
 * - Vector2.Angle liefert immer den kleinsten Winkel und geht damit von 0-180, nicht von 0-360
 * 
 * Altes PaintReservoir (1x1) aus dem Init()-Block in neuen Rakel injected, der ist aber größer (3x1) ...
 * 
 * Ein gesamtes 2D Array mit Objekten besetzen wollen. Aber vergessen, dass ich dafür nicht einfach nur die Referenz setzen darf, weil dann ändern sich ja immer alle Objekte, wenn eines sich ändert.
 */



/* IC Dokumentation
 * 
 * Arrange, Act, Assert
 * 
 * Erfahrungen mit TDD -> erlebte Situationen
 * - Interface vs Vererbung für Mocks
 * 
 * Test-Szenarien
 * - Cache + Test-First-Situation ...
 * - 
 * 
 * Rolle von Unit vs. Integrationstests
 * -> Integrationstests prüfen nur Aufrufe, decken also nicht alle Szenarien ab, die mit Unit-Tests behandelt werden
 * 
 * Viel weniger Debugging
 * Man weiß einfach der Code macht
 * Robustheit gegenüber Änderungen
 */



/* Log
 * 
 * - Input:
 *   - Maus-Position in Canvas-Space (float)
 *   - Textur-Objekt(e)
 * 
 * - Output:
 *   - bemalte(s) Textur-Objekt(e)
 *   
 * - Probleme:
 *   - man weiß einfach nicht was im Code tatsächlich passiert, wenn komplexe
 *     Vorgänge hinter ein einzelnes Interface gepackt werden
 *   - Debuggen ist extrem nervig, weil jedes Frame Update aufgerufen wird und
 *     alles mögliche gemacht wird
 *       - generell ist das real world szenario schwer nachvollziehbar
 *   - es ist nicht mal klar, ob es der Model-Code ist, der nicht funktioniert,
 *     oder ob an den Input-Parametern was nicht stimmt
 * 
 * - TDD Limitierung:
 *   - Funktion mit komplexem Algorithmus
 *      - Aufteilung in verschiedene private Funktionen
 *        -> die sollte man ja theoretisch nicht wirklich testen
 *           bzw. kann man ja auch nicht, weil sie eigentlich private sind
 * 
 * 
 * 14.06.2022
 * - Baut man ein Objekt als komplettes Model oder macht man alle nötigen Objekte
 *   aus denen das bestehen würden nach außen zum Controller sichtbar?
 *   
 * - Es ist extrem schwer, sich vorher ein Interface nach außen zu überlegen
 * 
 * 
 * 16.06.2022
 * // TODO return list of coordinates instead of 2D array
 * // -> more robust tests in case of changes
 * // -> array tight fit dimension problem is gone
 * 
 * Private Methods vs Static Helper Methods
 * - Private Methods -> Klassen haben alles was sie brauchen
 * - Static Helper Methods -> Testability, Hilfe beim Implementieren / Debuggen
 * 
 * Listen von Koordinaten vergleichen: Reihenfolge der Koordinaten relevant .......
 * 
 * 
 * 23.06.2022
 * Arrays als Speicher für Daten in Koordinatensystemen benutzen
 * - bei normalem Indexing arr[x,y], werden die Daten quasi um 90° nach rechts rotiert gespeichert
 * - also das normale Koordinatensystem mit Origin unten links wird um 90° nach rotiert gedreht
 * - Nutzung auf diese Art wichtig, um CPU-Cache zu benutzen
 * Nun ist die Frage wie die Maske eigentlich aussehen soll -> Ermitteln: Wie wird sie benutzt?
 * - Es wird durch alle Zeilen des Arrays [>#<, _] iteriert und geschaut, ob die Elemente gesetzt [_, >#<] sind oder nicht
 * -> also evtl. ist es doch besser die Maske richtig rum gedreht zu speichern
 * -> Lesezugriffe sind damit auch sicher sequentiell
 * -> Was heißt das?
 *  - Bresenham muss das Array so beschreiben können, dass Origin unten links ist (Koordinaten in Bresenham entsprechend transformieren)
 *      - Mapping von realen Koordinaten auf Array-Koordinaten
 *      - hängt ab von Größe des realen Koordinatensystems und Größe des Arrays
 *  - Scanline kann wieder für jede Zeile durchgeführt werden
 * 
 * aufgehört bei:
 * - Rechteck funktioniert
 * - Rotation kommt als nächstes
 * 
 * 
 * 26.06.2022
 * Arrays aus Farben vergleichen ist anstrengend ...
 * 
 * 
 * 30.06.2022
 * - Stand der Dinge
 *  - Maske ist irgendwie noch an x-Achse gespiegelt
 *  - Zeichnen sehr rechenaufwändig
 *      - Maske cachen
 *      - Maske nur anwenden, wenn die Position sich geändert hat
 *      - notfalls andere Repräsentation wählen, damit Anwendung der Maske effizienter wird
 *      
 *      
 * 01.07.2022
 * - 
 * 
 * 
 * 04.07.2022
 * - Maske optimieren, so dass das Ergebnis eine Sparse-Repräsentation ist wodurch das Apply um ein vielfaches schneller stattfinden kann
 *   - Scanline benötigt Zeilen -> Möglichkeiten für Datenstrukturen:
 *     - Array mit so vielen Zeilen wie es y-Werte gibt und zwei Spalten (x-Anfang, x-Ende)
 *       - es muss herausgefunden werden, wie viele y-Werte es gibt (Array-Erstellung)
 *         -> scheint machbar, ansonsten List verwenden?
 *       - es muss herausgefunden werden, wie die Werte der Maske auf den TextureSpace gemappt werden können
 *          - y-Koordinaten verraten wie weit wir vom Center entfernt sind
 *     - Dictionary mit einem Key für jede Zeile, Zeilen sind Arrays mit zwei Werten
 * - evtl. is es auch sinnvoller zuerst den bidirektionalen Farbaustausch zu implementieren, damit man dann weiß
 *   in welcher Form die Maske vorliegen soll
 *   
 *   
 * 05.07.2022
 * - Neue Implementierung für die Maske
 * 1. Koordinaten der Maske im InitialState berechnen
 * 2. Koordinaten um 0,0 rotieren
 * 3. Koordinaten an Stelle verschieben
 * 4. ApplyMask macht dann was für alle Koordinaten
 * 
 * 
 * 07.07.2022
 * -> es entstehen Löcher, siehe Grid.keynote
 * - es braucht also mehr Schritte
 * -> ApplyToCanvas macht dann:
 * 1. rotiertes Rechteck ausrechnen, auf dem Farben aufgetragen werden
 * -> aligned rectangle muss effizient berechnet werden
 * (2. Für jeden Pixel dieses Rechtecks auf den Farbspeicher in initialer Rotation mappen)
 * (-> Pixelkoordinaten zurückrotieren)
 * 3. Textur an Position <Rakelposition + Pixelkoordinate aligned rectangle> entspricht dann der zurückrotiert(Pixelkoordinate aligned rectangle)
 * -> Farbe auftragen / Farbe mitnehmen
 * 
 * Es gibt also die Komponenten:
 * - RectangleFootprint(Calculator)
 * - ColorReservoir + PickupMap
 * - RectangleFootprintMapper (auf ColorReservoir), evtl. reicht hier auch eine Funktion
 * 
 * Efficient Aligned Rectangle
 * - Repräsentation als 2D Array
 *   - So viele Zeilen wie y-Koordinaten
 *   - 2 Spalten, [0] x-Anfang, [1] x-Ende
 *   - Bresenham in dieses Array wird interessant
 * -> enthaltenene Optimierungen:
 *   - Effiziente Speichernutzung (nur Anfangs- und Endkoordinaten)
 *   - Einmalige Speicherallokation (Array, keine Vektoren)
 *   - Einsparung von Rechenschritten (nur Anfangs- und Endkoordinaten bedeutet, dass das Fill auf später verschoben werden kann, um es direkt mit einem weiteren Schritt zu verbinden)
 * 
 * aufgehört bei:
 * - Zeit für Berechnung und Anwendung der Maske messen
 * - Es scheint irgendeinen Bug zu geben, weil die Maske stets neu berechnet wird
 *   - Irgendwer setzt zwischendurch die Normale auf 0,0
 *   - Nein, ganz am Anfang ist sie 0,0 und dann bleibt sie bei 0,0 und natürlich ist dann Normal nie PreviousNormal ....
 *   - Hätte man einen Test gehabt, wär das vermutlich schon aufgefallen
 *
 *
 * 08.07.2022
 * Integrationstests (Rakel) vs Unittests (BasicMaskApplicator, ...)
 * - Integrationstests:
 *   + es ist egal, wie sich die Komponenten verändern, nur das Ergebnis wird geprüft
 *   - Bei Veränderung der Komponenten muss die Zusammensetzung des Rakels in den Tests angepasst werden
 *   - Man müsste für alle Variationen von Komponenten Tests haben
 * - Unittests
 *   + Komponenten lassen sich gut entwickeln
 *   - Bei Veränderungen der Komponenten (Rakelzusammensetzung) ist nicht unbedingt garantiert, dass noch alles funktioniert
 *   
 * Möglichkeiten für neues Mask-Interface:
 * - Array mit Zeilen aus Start + Ende
 *   + wenig Speicherbedarf + noch weiter optimierbar, so dass nur noch Zahlen verwendet werden im Array
 *   + Fill Operation fällt weg
 *   - Bresenham hierdrauf wird interessant (hoffentlich überhaupt effizient implementierbar)
 * - Liste aus allen Koordinaten
 *   + relativ einfach implementierbar
 *   - Fill Operation
 *   - mehr Speicherbedarf
 *   
 *   
 * aufgehört bei:
 * - evtl. wär Liste aus Koordinaten doch besser gewesen?
 *   -> Was ist effizienter: Vergleiche für x-Anfang und x-Ende oder viele new() Aufrufe für die Vektoren?
 * - FacedUp Case funktioniert nicht
 *   - angle ausgeben, Koordinaten ausgeben
 *   
 *   
 * 09.07.2022
 * OptimizedRakel erstmal fertig, buchstäblich 100x schneller
 * 
 * Test-After hat Nachteile
 * - Test richtet sich evtl. nur nach dem was man implementiert hat, nicht nach dem was man möchte
 *   (Reapply, Recalculate Mask Beispiel)
 *   
 * Brainstorming zu Farbaustausch:
 * - zwei Reservoirs auf dem Rakel -> Color[,] für die Farbe + int[,] für die Menge
 *   - PickupReservoir
 *   - ApplicationReservoir
 * - zunächst nur eine Farbschicht auf dem Canvas
 *   - langfristig:
 *     - Schichten: Farben
 *     - Schichent: Farbmengen
 *     - Schicht: NormalMap
 *     - alles in ein Color[] Array zwischenspeichern und am Ende jeweils übertragen
 *       - könnte Performanceprobleme geben
 *       - evtl. nur modifizierte Bereiche übertragen
 * - Farbreservoir alle -> Ausfaden muss modelliert werden
 *   - HSV? RGB?
 *     - bei RGB immer auf alle Farbkanäle noch was drauf addieren könnte funktionieren
 * - Wie war das noch mal mit der Farbmischung beim Auftrag?
 *   - TODO
 * 
 * - Design
 *   - FarbReservoirs im Rakel gespeichert
 *   - Applicator
 *      - bekommt ColorExchanger
 *        - ColorExchanger
 *           - bekommt Reservoirs
 *           - macht Mapping von Texturkoordinaten auf Reservoirs
 *           - macht Farbaustausch
 *     ODER
 *     - bekommt Reservoir
 *       - mit Interface
 *          - PickupFromPixel
 *          - ApplicateToPixel
 *       - hat ColorExchanger
 * 
 * 
 * ??.07.2022
 * Es wär eigentlich gut, wenn MaskApplicator nur erstmal die Mask auf Texture anwendet mit paintReservoir
 * der eigentliche Farbaustausch sollte woanders stattfinden, damit
 * - MaskApplicator testbar bleibt
 * - die ganze Farbaustausch-Logik nicht in den MaskApplicator Tests für jedes Szenario redundant getestet wird
 * >> PaintTransferManager/Operator
 *    - FromPickupMapToCanvas() -> Loop 2
 *    - FromCanvasToPickupMap() -> Loop 1
 * >> oder das ist die Aufgabe von PaintReservoir
 *    - Color Emit(res_x, res_y)
 *    - vod Pickup(res_x, res_y, canvas_color)
 *
 * MaskToReservoirMapper und ReservoirToMaskMapper werden auch benötigt
 * - p_x, p_y MapCanvasToPickupMap(c_x, c_y, mask_position, rakel_rotation)
 * - c_x, c_y MapPickupMapToCanvas(p_x, p_y, mask_position, rakel_rotation)
 * -> Mapping in MaskApplicator oder in PaintReservoir?
 *   - sollte PaintReservoir irgendwas über eine Maske wissen? Vermutlich nicht
 *
 * Tests:
 * - PaintReservoir
 *   - Pickup
 *   - Emit
 * - MaskApplicator
 *   - Pickup und Emit müssen auf den richtigen Koordinaten aufgerufen werden
 *   -> ?? Evtl. doch noch extra Abstraktion einführen?
 *     -> Hat Funktion DoPoint() und das macht dann das Mapping + Pickup + Emit?
 *     -> Das Problem mit DoPoint ist, dass es zwei Arten von DoPoint geben muss!!
 *     -> Zwischenlayer für beide Arten von DoPoint?
 *       -> hätte den Vorteil, dass das Testing für MaskApplicator nicht komplexer wird als es jetzt ist
 *       -> Andererseits gibt es dann für den Mechanismus als ganzes keinen Test mehr, ein Redesign wäre somit stets ein Risiko
 *     -> viele Unit Tests, trotzdem ein Integration Test mit vertretbarem Aufwand?
 *       -> Allerdings wird die Logik unter MaskApplicator vermutlich noch seeehr häufig angepasst werden
 *         -> Die Wartung dieser Integrationtests wäre einfach nur anstrengend
 *
 * - Mapper
 *   - ..
 *   - ..
 *
 * Eine Frage bleibt noch: Wer setzt die Farben in die Textur, und wer holt sie von dort?
 * Applicator oder Reservoir?
 * 
 * 
 * 27.07.2022
 * Überlegtes Design nicht super sinnvoll
 * - Die Koordinatentransformationen sollte evtl. der MaskApplicator machen, weil er sowieso die MaskPosition kennt
 *   - Außerdem hört es sich nicht sinnvoll an, dass das Reservoir eine Maske und ihre Position, sowie Rotation kennt
 * 
 * Tests für neue Version vom MaskApplicator schreiben (nun mit Farb-Reservoirs) sehr anstrengend
 * -> Was soll hier tatsächlich getestet werden? Nur genau das, was der MaskApplicator tut
 * -> Aber das ist teils schwer in Isolation zu testen, weil das Ergebnis nur mit den echten Komponenten rauskommt
 *   -> Ansonsten könnte ich ja auch einfach in den ColorMixerMock schreiben, dass er die vom Test gewünschte Farbe zurückgibt
 *   -> Oder ich mocke halt alle Komponenten und prüfe die Parameter für die Aufrufe genauer
 *     -> Dann ist die Frage ob es nicht einfacher ist, gleich alles durchzurechnen
 * -> Das muss ich dann ja aber für alle TestCases machen, oder aber nur für den einzelnen Pixel und bei den anderen überlege ich mir noch was einfacheres
 *   -> aber was ist einfach genug und dennoch sinnvoll zu testen?
 *   
 * String Log für Mocks für Unit Tests
 * - Vorteile:
 *   - Reihenfolge und Anzahl der Calls kann genau geprüft werden
 * - Nachteil:
 *   - Reihenfolge muss genau bestimmt werden! Dadurch leidet bei Arrays die Lesbarkeit der Tests
 *   
 * Next Steps:
 * - DONE PaintReservoir implementieren + testen
 * - OilPaintTexture erweitern + testen
 * - PaintTransfer Integration Tests implementieren
 * - TestRakel anpassen / löschen / kopieren?
 * - Kommentare aufräumen
 * 
 * 
 * 28.07.2022
 * OilPaintTexture erweitern + testen
 * - Abstraktion hinzufügen:
 *   - OilPaintSurface mit Interface
 *     - AddPaint
 *     - GetPaint
 *   - hat einen Member CustomTexture2D mit Interface
 *     - SetPixelFast
 *     - GetPixelFast
 *     - Apply
 *   - später wird das ganze sowieso noch erweitert, um mehrere Farbschichten zu simulieren
 *     - es wird dann also ein Zwischenarray geben, auf Basis dessen eine CustomTexture2D errechnet/modifiziert/geupdated wird
 *     - die Farbschichten könnten auch ein Member von OilPaintSurface sein
 *     - CustomTexture2D wird eine Abhängigkeit von OilPaintSurface sein, da das Texture2D-Objekt einmal als Textur für das material definiert wird
 *   - Oder einfach alles in OilPaintTexture machen? Welchen Vorteil hätte ein extra Objekt nur für SetPixelFast, GetPixelFast und Apply?
 *   - Aufgaben:
 *      - Farbmischung
 *      - Speicherung von Farbschichten
 *        + Übertragung in renderbare single-layered Farbschicht
 *      - Farbinitialisierung auf weiß
 *      - Apply Passthrough
 *      
 *      - SetPixelFast, GetPixelFast -> OOB Prüfung, Indizes ausrechnen
 *   -> OOB Prüfung wäre einfacher zu testen
 *     - wenn man das von außen durch OilPaintSurface wöllte, müsste man was tun?
 *     - AddPaint und GetPaint auf ungültigen Koordinaten aufrufen aber dann müsste man auch
 *       wieder genau wissen was man für die entsprechenden Aufrufe in Texture2D erwarten muss,
 *       d.h. für die Tests der OOB Prüfung muss man Implementierungsdetails in der Farbmischung kennen
 *       
 *   - Sollte OilPaintSurface ein Texture2D Objekt bekommen oder es selbst erstellen?
 *       
 * Testing:
 * - Was spricht dagegen einen Mock vom echten Objekt erben zu lassen? Evtl. ist
 *   es dann erforderlich auch die Konstruktorparameter des echten Objekts entgegenzunehmen
 *   - Wenn man aber ein Interface benutzt, dann kann man das neue Objekt nicht um öffentliche Methoden
 *     erweitern, da das Interface diese in C# offenbar erfordern würde !!?? Nvm, habe nur den Typen falsch hingeschrieben ... (OilPaintSurface statt OilPaintSurfaceMock)
 * - Probleme mit Interfaces für Mocks:
 *   - Genutzte Attribute mit Getter/Setter der Klasse werden unbenutzbar
 *   -> stimmt nicht, einfach den Getter im Interface definieren und beides noch mal in der Klasse mit gewünschten Rechten hinschreiben
 *   - Teilweise muss Verhalten gestubbed werden, z.B. für Getter, weil das Object Under Test diese bei der Initialisierung nutzt
 *   -> evtl. ist das mit der Initialisierung im Konstruktor auch einfach schlecht gelöst
 * 
 * Next Steps:
 * - DONE OilPaintTexture erweitern + testen
 *   - GetPixelFast Tests
 *   - OilPaintSurface Tests
 * - DONE Shared Mocks in extra Ordner schieben
 * - PaintTransfer Integration Tests implementieren
 * - TestRakel anpassen / löschen / kopieren?
 * - Rakel: SetColor -> FillColor
 * - Kommentare aufräumen
 * - Volume Implementierung
 * - Canvas Snapshot Buffer
 * 
 * 
 * 29.07.2022
 * Next Steps:
 * - ==DONE Neue Komponenten für PaintTransfer edge cases
 * - PaintTransfer Integration Tests implementieren
 * - TestRakel anpassen
 * - Rakel: SetColor -> FillColor
 * - Kommentare aufräumen
 * - Volume Implementierung
 * - Canvas Snapshot Buffer
 * 
 * Testing:
 * - Mocking erlaubt zwar das isolierte Testen einer Komponente
 * - Wird diese jedoch erweitert, müssen die Mocks wieder angepasst werden
 * - Die Komponente allerdings nicht isoliert zu testen (sondern quasi Integrationstests zu machen),
 *   bedeutet wiederum auch ein ständiges Anpassen der Tests solange die Komponente erweitert wird
 *   -> denn das Endergebnis verändert sich damit ja ständig
 * 
 * 
 * 10.08.2022
 * - Testing:
 *   - Test first hilft dabei zu merken, was die bestehenden Komponenten evtl. sogar schon können
 *      - OOB GetPaint ist z.B. schon erledigt, weil FastTexture2D bei OOB bereits NO_PAINT_COLOR zurückgibt
 * 
 * Next Steps:
 * - Volumen Implementierung für Farbreservoir
 *   - UI für Rakel Farbauffüllung
 *      - Predefined Colors
 *      - Colorpicker später
 *   - Rakel: UpdateColor löschen
 *   - Rakel: Fill Reservoir implementieren
 *   - PickupReservoir: Add / Set?
 *      - Add macht mehr Sinn, weil evtl. manchmal auch nur Farbe aufgenommen und keine abgegeben wird
 *      - Aber Add macht nur dann Sinn, wenn mehrere Farbschichten unterstützt werden ...
 *        (sonst wird ja bei jedem neuen Add die Farbe überschrieben)
 * - Volumen Implementierung für OilPaintSurface <--> Farbschichten Implementierung
 * - TestRakel anpassen
 * - ? PaintTransfer TestClass löschen
 * - Kommentare aufräumen
 * - OptimizedRakel -> Rakel
 * - Alle Rakelkomponenten in einen Ordner packen
 * - Anpressdruck beim über die Leinwand ziehen
 * - Canvas Snapshot Buffer
 * - Farbe ausfaden lassen wenn nur noch wenig Volume
 * - mehrere verschiedene Farbschichten auf den Rakel auftragen können
 * 
 * Aufgehört bei:
 * - Testing:
 *   - Integrationstests sind wichtig
 *     - Rakel: UpdateLength und UpdateWidth sollten auch das Reservoir neu anlegen ...
 *     
 * Sollte man Length und Width überhaupt updaten könnnen? Evtl. sollte eher ein neuer Rakel erstellt werden
 * -> Problem hiermit ist aber, dass es evtl. beim Benutzen anstrengend ist, weil man immer beachten muss,
 *    dass nach Length und Width Update noch mal Farbe aufgetragen werden muss
 * -> Wenn man das Reservoir behalten möchte, wird dies aber insbesondere bei schon verwendetem Rakel schwer
 *    nachvollziehbar, wo welche Farbe hinübertragen wurde
 * 
 * 
 * 12.08.2022
 * Probleme sind:
 * - Es kommt zu einer schnellen Verdünnung der Farbe, evtl. geht irgendwo welche verloren
 * - Ein leerer Rakel sollte auch Farbe über die Leinwand ziehen können
 * - Aufgenommene Farbe wird noch im selben Pixel wieder abgegeben
 *   -> Reihenfolge ändern bringt nichts, denn dann würde die abgegebene Farbe noch im selben Pixel wiederaufgenommen
 *   -> Canvas Snapshot Buffer
 *     - Umsetzung bei "rotierender Pickupmap"?
 *       - O wird nach jedem Imprint geupdated
 *         - jedoch nur für alle gerade geänderten Pixel, die nicht unter der neuen Maske liegen
 *     - Alternative Idee: Während Stroke haben alle veränderten Pixel eine "TTL" und erst nach deren Ablauf kann wieder Farbe abgegeben werden
 * 
 * Next Steps:
 * - Canvas Snapshot Buffer
 * - Bug: Durch die Farbmischung bei der Abgabe aus dem Reservoir geht immer die Hälfte der Farbe verloren
 * - Bug: Bei mehreren Klicks auf entfernten Flächen wird beim vierten Mal die Farbe halbiert
 * - PickupReservoir: Add
 * - Volumen Implementierung für OilPaintSurface <--> Farbschichten Implementierung
 * - Rakel ApplyToCanvas splitten und in UpdateNormal und UpdatePosition schieben?
 *   - Funktionen evtl. umbenennen
 *   - Es wird nie ein sinnvolles UpdatePosition ohne anschließendes Apply geben
 *   - Idee kam eigentlich, weil ich mich gefragt hab, wieso man dem Applicator Mask sowie MaskPosition und MaskNormal übergibt
 *     -> evtl. könnte man die beiden extra Attribute ja auch einfach in der Mask speichern
 *     
 * Canvas Snapshot Buffer:
 * - "Before the first imprint of a stroke, Ω is initialized to be identical to the current canvas map."
 *   -> Damit ist nicht vor jedem Strich hakt, am besten immer schon nach dem letzten Strich machen
 * - "Then, before every subsequent imprint, Ω is updated to contain the latest version of the canvas map except for the region covered by the pickup map at the current brush position"
 *   -> Für die Performance ist hierbei wichtig, immer nur die Änderungen zu übertragen
 *     - Übertragen werden müssen also die folgenden Pixel: Vorherig modifizierte Pixel MINUS Demnächst modifizierte Pixel
 * - "By using Ω as the input canvas map to our paint pickup update algorithm instead of the canvas itself, we avoid the tight feedback loop during the bidirectional paint transfer"
 *   -> GetPaint muss dann immer auf dem Canvas Snapshot Buffer gemacht werden
 *   - Farben für AddPaint und Rendering kommen immer noch direkt aus dem Surface
 * - Schritte im Mask Applicator:
 *   - Farbmitnahme aus Snapshot Buffer
 *   - Farbauftrag wie bisher
 *     - modifizierte Koordinaten (MK_1) speichern
 *   - Koordinaten MK_0 \ MK_1 in Snapshot-Buffer übertragen
 *     - das kann evtl. direkt in der nächsten Iteration erfolgen (immer direkt vor der Farbmitnahme aus dem Snapshot-Buffer)
 *       - dabei muss aber beachtet werden, dass nach der letzten Iteration alles übrige noch übertragn wird
 * 13.08.0222
 * - Design:
 *   - Alles in OilPaintSurface API versteckt
 *     - GetPaint nimmt immer aus dem Snapshot-Buffer
 *     - Neue Funktion: ImprintDone
 *       - überträgt alles was dazu gekommen ist MINUS eine Liste von Koordinaten in den Snapshot Buffer
 *   ODER
 *   - Handling komplett im MaskApplicator
 *     - SnapshotBuffer.GetPaint statt OilPaintSurface.GetPaint
 *       - SnapshotBuffer sollte OilPaintSurface-Referenz haben
 *         - um die GetPaint Aufrufe weiterzuleiten
 *         - aber nur, wenn an der Stelle im SnapshotBuffer überhaupt Farbe ist
 *       - TODO
 *         
 * Ablauf derzeit:
 * - oilPaintSurface.GetPaint
 * - paintReservoir.Pickup
 * - paintReservoir.Emit
 * - oilPaintSurface.AddPaint
 * 
 * - Snapshot Buffer verändert Schmierverhalten, derzeit gibt es aber gar kein Schmierverhalten
 *   -> evtl. das erstmal implementieren?
 *   - Es muss geregelt werden, dass die aufgenommene Farbe nicht im selben Pixel wieder abgegeben wird
 *     - Idee: kann eine Variation des SnapshotBuffers das evtl. auch leisten?
 *     - Keine Idee: Reihenfolge ändern:
 *       - Zuerst Farbe nitmehmen und dann abgeben: (derzeit)
 *         - Mitgenommene Farbe wird immer sofort wieder abgegeben
 *       - Zuerst Farbe abgeben und dann mitnehmen
 *         - Abgegebene Farbe wird immer sofort wieder mitgenommen
 *     - Idee: Neuer State in OilPaintSurface
 *       - StartImprint()
 *         - kopiert sich einmal das derzeitige Farbarray, damit dann bei den folgenden GetPaint-Aufrufen nicht die neue Farbe wieder mitgenommen wird
 *       - GetPaint nimmt dann immer die Farbe aus dem kopiertem Farbarray
 *         - Trotzdem muss irgendwie geregelt werden, dass die Farbe auf dem echten Array auch weniger wird
 *     - Idee: Alles über verzögerte Farbweiterleitung im MaskApplicator regeln
 *       - Problematik ist ja, dass aufgenommene Farbe nicht direkt wieder abgegeben werden soll
 *       - Also machen wir
 *         - oilPaintSurface.GetPaint -> Ergebnisse speichern
 *         - paintReservoir.Emit
 *         - oilPaintSurface.AddPaint
 *         - paintReservoir.Pickup aus gespeicherten Ergebnissen
 *           - für die Effizienz kann das auch immer vor den ersten Schritt geschoben werden, mit den Ergebnissen aus der letzten Iteration
 *           - dabei nicht vergessen: nach der letzten Iteration muss trotzdem Pickup gemacht werden
 *         -> verzögerte Farbaufnahme von der Leinwand
 *       - Oder andersherum:
 *         - paintReservoir.Emit -> Ergebnisse speichern
 *         - oilPaintSurface.GetPaint
 *         - paintReservoir.Pickup
 *         - oilPaintSurface.AddPaint aus gespeicherten Ergebnissen
 *         -> verzögerter Farbauftrag auf Leinwand
 *         
 *       - ließe sich dieses Konzept auch in einem speziellen SnapshotBuffer umsetzen?
 *         - Was macht der SnapshotBuffer überhaupt genau?
 *           - Die Farbe kommt aus einer Kopie der Leinwand
 *           -> quasi verzögerter Farbauftrag, denn das was jetzt aufgetragen wird, kann erst später wieder mitgenommen werden
 *         - Aber wie ist das geregelt, damit keine Farbe verloren geht/erzeugt wird?
 *           - Wenn die Farbe aus dem SnapshotBuffer aufgenommen wird, dann muss sie irgendwann auch von der Leinwand abgetragen werden
 *              - Und das muss dann auch genau die Farbe mit genau der Menge sein, wie soll das gehen?
 *              - Ablauf wäre:
 *                - Farbe aus SnapshotBuffer nehmen
 *                  - Aber wann wird diese Farbe von der Leinwand abgetragen?
 *                    - Angenommen sofort
 *                      welche Probleme träten auf?
 *                      - Man müsste die Farbe im SnapshotBuffer aus der Leinwandfarbe extrahieren,
 *                        denn diese hat sich durch vorheriges Auftragen vermutlich schon geändert
 *                      - Aber das wird zu jedem Zeitpunkt ein Problem sein, weil sich ja SnapshotBuffer und Canvas
 *                        von Natur aus immer unterscheiden
 *                    - Angenommen die Farbe wird bei jedem SnapshotBuffer Update bereits abgetragen
 *                      welche Probleme träten auf?
 *                      - Wie viel Farbe wird denn abgetragen? Das hinge später auch ja vom Anpressdruck ab
 *                      - Alle mitgenommene Farbe muss ja dann auch nach dem Imprint wieder aufgetragen werden
 *                        - Mischung mit aufgetragener Farbe???
 *                - Farbe in PickupMap packen
 *                - Farbe aus Reservoir nehmen
 *                - Farbe auf Canvas auftragen
 *                - SnapshotBuffer updaten
 *           - Alternative Implementierung?
 *             - SnapshotBuffer nur um zu tracken, an welchen Stellen es Unterschiede zwischen Leinwand und SnapshotBuffer gibt
 *               - Farbmitnahme nur dort, wo es TODO
 *     - Könnte die verzögerte Farbweiterleitung das Konzept des SnapshotBuffers nachbilden?
 *       - Verzögerung um k Schritte
 *       - Welche Art der Verzögerung eignet sich besser?
 *         - Farbaufnahme von der Leinwand - Probleme?
 *           - Wohin mit der übrigen Farbe nach Abschluss der letzten Iteration?
 *             - komplett ins Reservoir packen
 *             - in Queue behalten und erst mit nächstem Stroke ins Reservoir wandern lassen
 *         - Farbauftrag auf die Leinwand - Probleme?
 *           - Wohin mit der übrigen Farbe nach Abschluss der letzten Iteration?
 *             - komplett auf die Leinwand auftragen
 *             - in Queue behalten und erst mit nächstem Stroke auftragen
 *       - die Frage ist dann natürlich wie groß k sein sollte aber das ganze ist sehr 
 *         einfach implementierbar und evtl. wirksam
 *     - Umsetzung:
 *       - PickupPaintReservoir wird vollkommen eigener Typ
 *       - Basisdatenstruktur wird eine Queue
 *         - die wird am Anfang gefüllt mit k leeren Farbwerten
 *         
 * - Aufgehört bei:
 *   - das mit dem Delay funktioniert erstmal aber die Ergebnisse sind so naja
 *     - Evtl. noch was besseres als Array aus Queues überlegen
 *     - irgendwas funktioniert auch mit dem Pickup Mechanismus noch nicht ganz
 *       -> wenn ich einmal in Farbe klicke und danach mehrmals wonadershin, dann wird die Farbe nicht emitted
 *       -> gerade war das so, jetzt bekomm ich's aber nicht direkt reproduziert
 *       -> dennoch wird die Farbe vom Canvas manchmal nicht direkt mitgenommen/gelöscht
 *       -> Oder sie wird aus irgendeinem Grund doch wieder sofort aufgetragen
 *       -> aber es scheint so, als wenn die Farbe irgendwie nur einmal mitgenommen wird
 *         -> aber ist ja klar:
 *           - nach dem ersten mal ist die Queue ja wieder nur eins lang und damit wird die
 *             Farbe tatsächlich wieder sofort aufgetragen
 *       -> "Fixed-Size Queue" die jede Farbe durchlaufen muss, bis sie im Reservoir landet
 *          (welches dann evtl. einfach mehrschichtig sein sollte)
 *          -> alte Klassenhierarchie wieder herstellen ....
 * 14.08.2022
 * Wie soll das Pickup-Modell funktionieren?
 * - Farbreservoir mit Schichten?
 *   - Stack oder Queue für die Schichten?
 *   - Wie viele Schichten sind erlaubt?
 *     - Unbegrenzt wäre einfach
 *       - aber unrealistischer
 *       - andererseits ist die Frage, wie viele Farbschichten sich überhaupt ansammeln können,
 *         denn es wird ja in jedem Schritt auch wieder Farbe abgegeben
 *     - Begrenzt würde bedeuten, dass man dann bei kompletter Befüllung auch keine Farbe mehr in
 *       die Pickup-Pipeline packen darf
 *     - Kommt es überhaupt zur Schichtenbildung?
 *       - Jedes Mal wenn Farbe eingequeued wird, wird ja auch wieder die verfügbare Schicht abgegeben
 *       - Schichtenbildung also nur durch den Delay
 * - einzelne Farbschicht
 *   - gleiches Problem wie mit begrenzter Anzahl Schichten
 * 
 * Pickup-Modell Umsetzung
 * - 3D Array
 *   - für jedes Pixel ein Array mit k Elementen
 *   - Farbe muss immer alle Positionen durchlaufen um wieder abgegeben zu werden
 *     - Quasi Queue mit fixed Length
 *   - Emit nimmt immer von ganz vorn
 *   - Pickup verursacht Weiterrutschen
 *     
 *   - Randfall: Rakel nicht vollständig auf dem Bild TODO
 *     - Farbe darf in diesen Fällen im Reservoir nicht weiterrutschen
 *     - Es muss dann für das PaintReservoir evtl. doch einen Unterschied zwischen NO_PAINT_COLOR und OOB geben
 *       - das eine für keine Farbe auf dem Canvas -> Weiterrutschen sinnvoll
 *       - das andere für keine Farbe weil OOB -> Weiterrutschen darf nicht passieren, denn es wurde ja keine Farbe emitted
 *       - ODER Mask-Applicator ruft erst gar nicht Pickup auf, aber dann muss auch hier noch extra herausgefunden werden,
 *              ob das gerade OOB ist
 * - Snapshot Buffer wird auch nur bedingt simuliert bei k Schritten, denn es erfolgt ja eine
 *   kontinuierliche Absorption
 *   -> nur keine sofortige Wiederabgabe
 * Testing
 * - FarbDelay im PickupReservoir in extra Komponente auslagern?
 *   - Tests werden sonst zu komplex I think ...
 * 15.08.2022
 *   - DelayBuffer, hat PickupReservoir?
 *     - oder DelayedPickupReservoir -> macht das die Tests einfacher though?
 *     - oder BufferLogik in RakelPaintReservoir?
 * - Das mit den Tests ist hier sowieso so eine Sache, weil aktuell ja nur die gesamte Komponente RakelPaintReservoir getestet wird
 *
 * Wie überhaupt Delay implementieren?
 * - Aktuell:
 *   - Queue:
 *     NO_PAINT_COLOR
 *     pickup
 *     FARBE1 NO_PAINT_COLOR
 *     emit
 *     FARBE1
 *     pickup FARBE2
 *     FARBE2 FARBE1
 *     emit
 *     FARBE2
 *   ODER Specialcase:
 *     NO_PAINT_COLOR
 *     pickup FARBE1
 *     FARBE1 NO_PAINT_COLOR
 *     emit
 *     FARBE1
 *     pickup NO_PAINT_COLOR
 *     FARBE1
 *     Queue leer
 *     _
 *   ->> ReservoirQueue wird bei leeren Farben nicht befüllt
 *   - Warum nehmen wir leere Farben noch einmal nicht auf?
 *     -> bisher würde das ja einfach die Farbe überschreiben
 *     -> und es macht auch keinen Sinn eine leere Farbe in eine Queue zu packen (leere Schicht macht keinen Sinn)
 *       -> außer eben um einen Delay zu verursachen
 *       
 * Hätten wir das gefixt, nun ist der Effekt
 * - dass sich die Farbe bei Delay 1 auf dem Rakel entlangkopiert
 *   -> da sehen wir also, wieso das mit dem Snapshot-Buffer Sinn macht
 * - Das wäre evtl. durch einen größeren Delay noch regelbar aber auch unrealistisch ist, dass
 *   die Farbe manchmal flächig vollständig vom Rakel absorbiert/mitgenommen wird
 * - Ein zu großer PickupDelay führt auch dazu, dass nach "Strich"-Ende erstmal eine Weile
 *   keine Farbe mehr abgegeben wird, aber plötzlich dann doch wieder
 * - Pixel werden ja beim Anwenden auch übersprungen, was vermutlich zu unschönen
 *   Streifen führt
 * - ein Snapshot-Buffer wird nicht wirklich implementiert, denn die Farbe wird ja
 *   trotzdem die ganze Zeit mitgenommen
 * 
 *   
 *   
 * 16.08.2022
 * Canvas Snapshot Buffer übehaupt sinnvoll?
 * -> im Endeffekt sorgt er dafür, dass nur "alte" Farbe mitgenommen wird, alles was sich unter dem Beginn
 *    der Impression befindet, wird aber liegengelassen
 * -> nee stimmt nicht, denn nur die neu aufgetragenene Farbe wird ja nicht in den SnapshotBuffer übetragen
 * - es wär wirklich interessant, wie man den SnapshotBuffer wirklich implementieren könnte
 * 
 * Next Steps:
 * - Pixel für Pixel über Canvas ziehen
 * - Canvas Snapshot Buffer
 * - Bug: Durch die Farbmischung bei der Abgabe aus dem Reservoir geht immer die Hälfte der Farbe verloren
 * - PickupReservoir: Farbschichten?
 * - Volumen Implementierung für OilPaintSurface <--> Farbschichten Implementierung <--> Farbschichten + Volumen
 *   - es kommt sonst vor, dass Pickup alles mitnimmt, was sehr unnatürlich aussieht
 * - Rakel ApplyToCanvas splitten und in UpdateNormal und UpdatePosition schieben?
 *   - Funktionen evtl. umbenennen
 *   - Es wird nie ein sinnvolles UpdatePosition ohne anschließendes Apply geben
 *   - Idee kam eigentlich, weil ich mich gefragt hab, wieso man dem Applicator Mask sowie MaskPosition und MaskNormal übergibt
 *     -> evtl. könnte man die beiden extra Attribute ja auch einfach in der Mask speichern
 * - Irgendwas überlegen, damit sich die Farbe auch auf dem Reservoir verschiebt?
 *     
 * Ideen zum Canvas Snapshot Buffer
 * - Verzögerung andersherum, also ins OilPaintSurface hinein
 *   - Problem: Wie lang wählt man den Buffer?
 *   - 25.08. Problem: Ist das überhaupt effizient implementierbar?
 *     - bei der anderen Lösung könnte man für jeden Pixel solange die Schichten durchgehen, bis man auf pickupable Paint trifft
 * - Paint hat die Eigenschaft "pickupable" und die wird immer erst auf true gesetzt, wenn die Maske sich
 *   vom Pixel wegbewegt hat
 *   -> so könnte man den CSB exakt nachbilden (?)
 *   - Problem: Können dann untere Farbschichten trotzdem mitgenommen werden?
 *   - Wie ist das überhaupt? Werden die Farben beim Auftragen auch mit den Farben auf dem Surface gemischt?
 * - Ist halt die Frage, wie schlimm es ist, dass eben aufgetragene Farbe auch wieder mitgenommen werden kann
 *   Wenn das kein Problem ist, wäre Variante 1 vermutlich leichter zu implementieren
 *   
 * 
 * 17.08.2022
 * Pixel für Pixel über Canvas ziehen:
 * - Neue Benutzung des Rakels:
 *   - StartStroke
 *   - UpdatePosition/UpdateNormal <- Apply
 *   - UpdatePosition/UpdateNormal <- Interpolieren, Apply for All
 *   - ...
 *   - EndStroke <- Last Position kann gelöscht werden (oder das macht man beim nächsten StartStroke)
 * - NewStroke vs MoveTo and LineTo
 * - zu schnelle Bewegung hakt dann doch, aber das wird wohl ohne GPU nur schwer vermeidbar sein
 * - Extra Layer nur für Position / Normal Interpolierung?
 *   - Rakel schaut dann nur, wann eine Maske neu berechnet werden muss
 *   - RakelInputInterpolator hat Rakel?
 *     - NewStroke()
 *     - Update(position, normal)
 *   - Rakel:
 *     - UpdateNormal(normal)
 *     - ApplyAt(position)
 *   -> erstmal schauen wie die Tests aussehen, wenn wir keine extra Komponente haben
 *     - das mit der Normale ist halt schon "interessant" zu testen, wenn wir einen vollen Integrationstest machen
 *       - sehr schwer isoliert von Pickup/Emit Logik zu testen
 *   -> eine extra Komponente wäre sinnvoll, aber wie machen wir das mit dem Design?
 *     - Rakel wird ja direkt aus OilPaintEngine benutzt für z.B. UpdatePaint
 *     - jetzt eine neue Komponente zu schaffen, die dann nur für die Kommunikation mit Rakel für UpdatePosition, ...
 *       zuständig ist, scheint merkwürdig
 *       -> aber vielleicht trotzdem viable?
 *     - Rakel könnte einen Interpolator haben und den dann benutzen
 *       - Interface zum Interpolator? NewStroke() call durchreichen wär jetzt nicht so schön
 * 
 * Testing:
 * - Asserting direct public side effects vs calls on a mock [Rakel UpdateNormal]
 * -> direct public side effect
 *    - wäre umfassender, denn man muss dann genau wissen wie sich die Textur verändert
 *    - außerdem teilweise schwer zu implementieren, da man das Ergebnis ausrechnen müsste
 *      was sich dann evtl. wieder ständig ändert -> schwer zu warten
 *    - dafür ist der Test robuster gegenüber Implementierungsdetails, wie z.B. dass es
 *      überhaupt einen MaskCalculator gibt
 * -> calls on a mock
 *    - Test ist kurz und leicht zu verstehen
 *    - nicht so robust gegenüber Änderungen des Designs
 *    
 * 18.08.2022
 * Also neue Komponente: RakelDrawer
 * 
 * Dann können wir aber auch gleich überlegen, ob wir FillPaint nicht auch auf dem Reservoir direkt machen
 * (dann müssten wir das aber auch injecten)
 * 
 * Nach Umbau:
 * - PaintReservoir in Rakel injecten
 * - IComponent -> ComponentInterface
 *   -> nee doch nicht, sieht im Code nicht gut aus und außerdem ist die Sortierung in der
 *      Datei-Anzeige immer noch nicht perfekt, weil RakelA vor RakelInterface kommt ...
 *      
 * - Rakel ApplyToCanvas splitten und in UpdateNormal und UpdatePosition schieben?
 *   - Funktionen evtl. umbenennen
 *   - Es wird nie ein sinnvolles UpdatePosition ohne anschließendes Apply geben
 *   - Idee kam eigentlich, weil ich mich gefragt hab, wieso man dem Applicator Mask sowie MaskPosition und MaskNormal übergibt
 *     -> evtl. könnte man die beiden extra Attribute ja auch einfach in der Mask speichern
 *     -> Rakel muss sowieso Normal speichern, weil er herausfinden muss, ob für die Normale schon mal eine Mask
 *        ausgerechnet wurde
 * 
 * Next Steps:
 * - Canvas Snapshot Buffer (oder Delay in AddPaint auf OilPaintSurface)
 * - Bug: Durch die Farbmischung bei der Abgabe aus dem Reservoir geht immer die Hälfte der Farbe verloren
 * - PickupReservoir: Farbschichten? oder Farbmischung + Volumen
 * - Volumen Implementierung für OilPaintSurface <--> Farbschichten Implementierung <--> Farbschichten + Volumen
 *   - es kommt sonst vor, dass Pickup alles mitnimmt, was sehr unnatürlich aussieht
 * - Irgendwas überlegen, damit sich die Farbe auch auf dem Reservoir verschiebt?
 * 
 * Farbmodell:
 * - Canvas:
 *   - Farbschichten, Volumen wird durch mehrere Schichten derselben Farbe erreicht
 *   - Farbmischung bei Auftrag?
 *     - Eventuell nicht nötig
 *       - Farbmischung passiert schon bei Pickup und Application
 *       - Alpha-Blending in den oberen Farbschichten
 *   - Wetness
 * - PaintReservoir:
 *   - PickupReservoir: Mischung der Farben + Volumenangabe
 *   - ApplicationReservoir: Farbe + Volumen
 * - Verhindern von zu schnellem bidirektionalem Farbaustausch:
 *   - Delay in OilPaintSurface implementieren
 *     -> vermutlich besser implementierbar, siehe 16.08.2022 -> Ideen zum Canvas Snapshot Buffer
 * - Rendering
 *   - obere Schicht bekommt Alpha?
 *   - Normalmap aus Volumen bilden
 *   
 * 
 * 19.08.2022
 * Next Steps:
 * - Winklige Rakel:
 *   - Reservoir ist bestimmt eher leer
 *   - aber trotzdem müsste auf dem Canvas eine gefüllte Fläche erscheinen (mindestens das erste Mal)
 * - UI Normal wird immer erst später geupdated
 * - Canvas Snapshot Buffer (oder Delay in AddPaint auf OilPaintSurface)
 * - Bug: Durch die Farbmischung bei der Abgabe aus dem Reservoir geht immer die Hälfte der Farbe verloren
 * - PickupReservoir: Farbschichten? oder Farbmischung + Volumen
 * - Volumen Implementierung für OilPaintSurface <--> Farbschichten Implementierung <--> Farbschichten + Volumen
 *   - es kommt sonst vor, dass Pickup alles mitnimmt, was sehr unnatürlich aussieht
 * - Irgendwas überlegen, damit sich die Farbe auch auf dem Reservoir verschiebt?
 * - GUI: Rotation für gegebene Strichlänge ermöglichen (Winkel_Anfang, Winkel_Ende, Strichlänge)
 * 
 * Testing:
 * - Rakel.UpdateNormal macht einen Call auf den MaskCalculator was ich getestet habe
 * - nicht getestet habe ich aber, ob dann auch die neue Normale verwendet wird um die Maske zu berechnen
 * -> test direct public side effects ist hier evtl. doch sinnvoller?
 * -> test direct public side effects aber nicht im vollständigen Sinn mit Ergebnis auf OilPaintSurface
 *    sondern nur auf welchen Pixeln hier Apply angewandt wird
 * -> oder man testet halt auch noch, welche Normal verwendet wurde im MaskCalculator Call
 * -> oder man macht einen Spy MaskApplicator, der die Normale aus der Maske herausfindet
 * 
 * ca. 850 Zeilen Code
 * ca. 1650 Zeilen Testcode
 * davon ca. 260 Zeilen Testcode nur Array-Formatierung
 * 
 * Next Steps:
 * - Winklige Rakel:
 *   - Reservoir ist bestimmt eher leer
 *   - aber trotzdem müsste auf dem Canvas eine gefüllte Fläche erscheinen (mindestens das erste Mal)
 * - Canvas Snapshot Buffer (oder Delay in AddPaint auf OilPaintSurface)
 * - Bug: Durch die Farbmischung bei der Abgabe aus dem Reservoir geht immer die Hälfte der Farbe verloren
 * - PickupReservoir: Farbschichten? oder Farbmischung + Volumen
 * - Volumen Implementierung für OilPaintSurface <--> Farbschichten Implementierung <--> Farbschichten + Volumen
 *   - es kommt sonst vor, dass Pickup alles mitnimmt, was sehr unnatürlich aussieht
 * - Irgendwas überlegen, damit sich die Farbe auch auf dem Reservoir verschiebt?
 * - GUI: Rotation für gegebene Strichlänge ermöglichen (Winkel_Anfang, Winkel_Ende, Strichlänge)
 * - Bug bei schrägem Rakel:
 * 
 *   System.Threading.Tasks.Parallel.For (System.Int32 fromInclusive, System.Int32 toExclusive, System.Action`2[T1,T2] body) (at <e38a6d3ee47c43eb9b2e49c63fc0aa48>:0)
 *   MaskApplicator.Apply (Mask mask, UnityEngine.Vector2Int maskPosition, IOilPaintSurface oilPaintSurface, IRakelPaintReservoir paintReservoir) (at Assets/Scripts/Rakel/MaskApplicator.cs:80)
 *   Rakel.ApplyAt (UnityEngine.Vector2Int position, System.Boolean logMaskApplyTime) (at Assets/Scripts/Rakel/Rakel.cs:56)
 *   RakelDrawer.AddNode (UnityEngine.Vector2Int position, UnityEngine.Vector2 normal, System.Boolean logTime) (at Assets/Scripts/Rakel/RakelDrawer.cs:75)
 *   OilPaintEngine.Update () (at Assets/Scripts/OilPaintEngine.cs:94)
 *   
 *   InvalidOperationException: Queue empty.
 *   System.Collections.Generic.Queue`1[T].Dequeue () (at <e38a6d3ee47c43eb9b2e49c63fc0aa48>:0)
 *   PickupPaintReservoir.Emit (System.Int32 x, System.Int32 y) (at Assets/Scripts/Rakel/PaintReservoir/PickupPaintReservoir.cs:38)
 *   RakelPaintReservoir.Emit (System.Int32 x, System.Int32 y) (at Assets/Scripts/Rakel/PaintReservoir/RakelPaintReservoir.cs:48)
 *   MaskApplicator+<>c__DisplayClass1_0.<Apply>b__1 (System.Int32 i, System.Threading.Tasks.ParallelLoopState state) (at Assets/Scripts/Rakel/MaskApplicator.cs:98)
 * - Bug: nach Rotation geht das FarbReservoir irgendwie schneller leer als vorher, auch wenn man wieder zurückrotiert
 *   - generell ist dann nach Zurückrotation auch ewig Farbe im Rakel ...
 * 
 * 
 * 24.08.2022
 * Idee für Mapping-Probleme: Vielleicht gar nicht versuchen ein volles Mapping zu erreichen.
 *                            Dann gibt es halt Unregelmäßigkeiten, ist vielleicht gar nicht schlimm.
 *                            
 * Fließende Übergänge bei unterschiedlich viel Farbe -> erfordert Alpha-Blending oder ähnliches
 * 
 * Bug:
 * - neue Rakelmaße füllen den Rakel wieder mit Farbe
 * - Woher kommen die Streifen?
 *   -> vermutlich werden die EMPTY_COLOR Farben in der Pickup Queue mit der Application Farbe vermischt
 *   -> kann aber eigentlich nicht sein, evtl. ist es auch der Pickup-Mechanismus der die Farbe ja immer noch sofort mitnehmen kann
 * 
 * Beim Verschmieren muss erstmal mehr Farbe mitgenommen als wieder abgegeben wird
 * - oder halt je nach Anpressdruck
 * 
 * Alle Farben im Reservoir zusammenmischen und Volumen addieren
 * 
 * 
 * 12.09.2022
 * TODO
 * - IntegrationTests für RakelDrawer, sonst ist aktuell nicht geklärt, ob beim Apply-Call auch OPS weitergegeben wird
 * 
 * 13.09.2022
 * Next Steps:
 * - Winklige Rakel:
 *   - Anteiliges Emit aus dem Reservoir implementieren
 *   - irgendwie geht die Farbe beim Pickup verloren
 *     - nein, sie wird nur nicht wieder abgegeben, weil das mit der zurück-Rotation dann genau nicht hinhaut
 *     -> anteiliges Emit löst dieses Problem
 * - Volumen für Emit und Pickup steuerbar machen, aktuell wird sonst von der Farbmischung im Reservoir kein Gebrauch gemacht, denn dort kann nie mehr als 1 Stück Farbe liegen
 * - IntegrationTests für RakelDrawer, sonst ist aktuell nicht geklärt, ob beim Apply-Call auch OPS weitergegeben wird
 * - Canvas Snapshot Buffer (oder Delay in AddPaint auf OilPaintSurface)
 * - Bug: Durch die Farbmischung bei der Abgabe aus dem Reservoir geht immer die Hälfte der Farbe verloren
 * - PickupReservoir: Farbschichten? oder Farbmischung + Volumen
 * - Volumen Implementierung für OilPaintSurface <--> Farbschichten Implementierung <--> Farbschichten + Volumen
 *   - es kommt sonst vor, dass Pickup alles mitnimmt, was sehr unnatürlich aussieht
 * - Irgendwas überlegen, damit sich die Farbe auch auf dem Reservoir verschiebt?
 * - GUI: Rotation für gegebene Strichlänge ermöglichen (Winkel_Anfang, Winkel_Ende, Strichlänge)
 * 
 * 
 * 16.09.2022
 * GPU-Beschleunigung?
 * - https://docs.unity3d.com/ScriptReference/GraphicsBuffer.html
 * - https://docs.unity3d.com/ScriptReference/ComputeShader.html
 * - https://www.youtube.com/watch?v=dhVJE7g3hig
 * 
 * Millisekunden je Abdruck messen
 * 
 * 
 * 07.10.2022
 * - Farbreservoir
 *   - Als Stack (mehrfarbig)
 *   - Als Array mit Volumenwerten (einfarbig)
 * - Menge der übertragenen Farbe
 *   - abhängig von Volumen
 *   - abhängig von Abdrucksverzerrung
 *   
 *   
 * 24.10.2022
 * ComputeShaders
 * - https://www.youtube.com/watch?v=BrZ4pWwkpto
 * - ComputeBuffers
 * - HLSL
 * 
 * 
 * 08.11.2022
 * Testing:
 * - manche Features (Volumenimplementierung) erfordern erst Anpassungen am Design und Testdesign
 *   - TestRakelPaintResevoir hat viel zu viel getestet, was sehr schwer durchschaubar und anpassbar gewesen wäre
 *   - Das Redesign der Tests musste vor der Volumenimplementierung geschehen, gleichzeitig mit der Volumenimplementierung wäre es zu unübersichtlich geworden
 *
 *
 * 09.11.2022
 * Testing
 * - für manche neuen Features müssen seeehr viele Tests angepasst werden (Color -> Paint)
 * -> alle direkt betroffenen auskommentieren
 * -> alle indirekt betroffenen (andere Komponenten) einfach nachziehen mit erstem direkt betroffenen Testcase
 * -> Fehler eingebaut aber auch gefunden durch Tests
 * 
 * TODO
 * absolute vs relative Volumenwerte
 * 
 * Next Steps:
 * - Volumen Implementierung für OilPaintSurface <--> Farbschichten Implementierung <--> Farbschichten + Volumen
 *   - es kommt sonst vor, dass Pickup alles mitnimmt, was sehr unnatürlich aussieht
 * - Bug: nur schmale Streifen bei jedem zweiten Zug
 * - Bug: Egal wie viel Volumen auf der Rakel ist -> es reicht immer für die gleiche Strecke
 * - TestOilPaintSurface_RakelView noch in dieser Form benötigt?
 * - Winklige Rakel:
 *   - Anteiliges Emit aus dem Reservoir implementieren
 *   - irgendwie geht die Farbe beim Pickup verloren
 *     - nein, sie wird nur nicht wieder abgegeben, weil das mit der zurück-Rotation dann genau nicht hinhaut
 *     -> anteiliges Emit löst dieses Problem
 * - Volumen für Emit und Pickup steuerbar machen, aktuell wird sonst von der Farbmischung im Reservoir kein Gebrauch gemacht, denn dort kann nie mehr als 1 Stück Farbe liegen
 * - IntegrationTests für RakelDrawer, sonst ist aktuell nicht geklärt, ob beim Apply-Call auch OPS weitergegeben wird
 * - Canvas Snapshot Buffer (oder Delay in AddPaint auf OilPaintSurface)
 * - Irgendwas überlegen, damit sich die Farbe auch auf dem Reservoir verschiebt?
 * - GUI: Rotation für gegebene Strichlänge ermöglichen (Winkel_Anfang, Winkel_Ende, Strichlänge)
 * 
 * 
 * 10.11.2022
 * Next Steps:
 * - Volumen Implementierung für OilPaintSurface <--> Farbschichten Implementierung <--> Farbschichten + Volumen
 *   - es kommt sonst vor, dass Pickup alles mitnimmt, was sehr unnatürlich aussieht
 *   - absolute vs relative Volumenwerte
 * - Bug: Irgendwo wird schwarze Farbe ins Reservoir gemischt und abgegeben
 *        -> passiert beim Wischen im 45° Winkel über schon aufgetragene Farbe
 *        -> nach dem Zurückrotieren auf 0° kommen dann die schwarzen Streifen
 * - TestOilPaintSurface_RakelView noch in dieser Form benötigt?
 * - Winklige Rakel:
 *   - Anteiliges Emit aus dem Reservoir implementieren
 *     -> Multithreading könnte hier zum Problem werden, weil nicht geklärt ist,
 *        welcher Thread die Farbe aus den Nachbarpixeln zuerst bekommt
 *   - irgendwie geht die Farbe beim Pickup verloren
 *     - nein, sie wird nur nicht wieder abgegeben, weil das mit der zurück-Rotation dann genau nicht hinhaut
 *     -> anteiliges Emit löst dieses Problem
 * - Volumen für Emit und Pickup steuerbar machen, aktuell wird sonst von der Farbmischung im Reservoir kein Gebrauch gemacht, denn dort kann nie mehr als 1 Stück Farbe liegen
 * - Canvas Snapshot Buffer (oder Delay in AddPaint auf OilPaintSurface)
 * - Irgendwas überlegen, damit sich die Farbe auch auf dem Reservoir verschiebt?
 * - GUI: Rotation für gegebene Strichlänge ermöglichen (Winkel_Anfang, Winkel_Ende, Strichlänge)
 * 
 * Testing:
 * - Integration-Tests müssen simpel gehalten werden aber nicht zu simpel
 * -> Bug mit der gleichen Referenz aller Paint-Objekte im Reservoir ist nicht aufgefallen, weil das Reservoir
 *    im Integrationtest nur 1x1 groß war
 *    
 * 11.11.2022
 * Next Steps:
 * - Streifenbug finden
 * - Zeichnen asynchron ausführen
 * - Zusammenhang zwischen Neigungswinkel und Menge der Farbe untersuchen
 * - NormalMap über Sobel-Filter (oder doch irgendwie BumpMap in den Shader schieben?)
 * - Painting-Knife Paper lesen und beschreiben
 * - Volumen Implementierung für OilPaintSurface <--> Farbschichten Implementierung <--> Farbschichten + Volumen
 *   - es kommt sonst vor, dass Pickup alles mitnimmt, was sehr unnatürlich aussieht
 *   - absolute vs relative Volumenwerte
 * - Bug: Irgendwo wird schwarze Farbe ins Reservoir gemischt und abgegeben
 *        -> passiert beim Wischen im 45° Winkel über schon aufgetragene Farbe
 *        -> nach dem Zurückrotieren auf 0° kommen dann die schwarzen Streifen
 * - TestOilPaintSurface_RakelView noch in dieser Form benötigt?
 * - Winklige Rakel:
 *   - Anteiliges Emit aus dem Reservoir implementieren
 *     -> Multithreading könnte hier zum Problem werden, weil nicht geklärt ist,
 *        welcher Thread die Farbe aus den Nachbarpixeln zuerst bekommt
 *     -> Parallel For genauer untersuchen
 *     -> Sieht aus wie RangedPartitioning https://devblogs.microsoft.com/pfxteam/partitioning-in-plinq/
 *   - irgendwie geht die Farbe beim Pickup verloren
 *     - nein, sie wird nur nicht wieder abgegeben, weil das mit der zurück-Rotation dann genau nicht hinhaut
 *     -> anteiliges Emit löst dieses Problem
 * - Volumen für Emit und Pickup steuerbar machen, aktuell wird sonst von der Farbmischung im Reservoir kein Gebrauch gemacht, denn dort kann nie mehr als 1 Stück Farbe liegen
 * - Canvas Snapshot Buffer (oder Delay in AddPaint auf OilPaintSurface)
 * - Irgendwas überlegen, damit sich die Farbe auch auf dem Reservoir verschiebt?
 * - GUI: Rotation für gegebene Strichlänge ermöglichen (Winkel_Anfang, Winkel_Ende, Strichlänge)
 * 
 * 
 * 17.11.2022
 * Next Steps:
 * - Streifenbug finden
 *   - Farbe wird aufgetragen (aber nur eine Schicht)
 *   - beim nächsten Schritt alles außer wieder mitgenommen außer die letzte Position (das ist dann ein Streifen)
 *   - beim nächsten Schritt wieder nur eine Schicht aufgetragen
 *   - usw.
 *   -> eher Rendering Problem
 * - Zeichnen asynchron ausführen
 * - Zusammenhang zwischen Neigungswinkel und Menge der Farbe untersuchen
 * - NormalMap über Sobel-Filter
 *   - Edge Cases müssen noch gemacht werden
 *   - Tests + Test für Apply Call
 *   - Thread Safety?
 *   - schräges Ziehen macht Stufen -> das passiert, weil man für einen Pixel nur nach oben zieht -> da liegt dann mehr Farbe
 *   - evtl. noch sobel_x und sobel_y invertieren?
 *      -> komischerweise nur sobel_y
 *   - Wert für z und scale finden
 * - Painting-Knife Paper lesen und beschreiben
 * - Volumen Implementierung für OilPaintSurface <--> Farbschichten Implementierung <--> Farbschichten + Volumen
 *   - es kommt sonst vor, dass Pickup alles mitnimmt, was sehr unnatürlich aussieht
 *   - absolute vs relative Volumenwerte
 * - TestOilPaintSurface_RakelView noch in dieser Form benötigt?
 * - Winklige Rakel:
 *   - Anteiliges Emit aus dem Reservoir implementieren
 *     -> Multithreading könnte hier zum Problem werden, weil nicht geklärt ist,
 *        welcher Thread die Farbe aus den Nachbarpixeln zuerst bekommt
 *     -> Parallel For genauer untersuchen
 *     -> Sieht aus wie RangedPartitioning https://devblogs.microsoft.com/pfxteam/partitioning-in-plinq/
 *   - irgendwie geht die Farbe beim Pickup verloren
 *     - nein, sie wird nur nicht wieder abgegeben, weil das mit der zurück-Rotation dann genau nicht hinhaut
 *     -> anteiliges Emit löst dieses Problem
 * - Volumen für Emit und Pickup steuerbar machen, aktuell wird sonst von der Farbmischung im Reservoir kein Gebrauch gemacht, denn dort kann nie mehr als 1 Stück Farbe liegen
 * - Canvas Snapshot Buffer (oder Delay in AddPaint auf OilPaintSurface)
 * - Irgendwas überlegen, damit sich die Farbe auch auf dem Reservoir verschiebt?
 * - GUI: Rotation für gegebene Strichlänge ermöglichen (Winkel_Anfang, Winkel_Ende, Strichlänge)
 * 
 * 
 * 18.11.2022
 * Aufgehört bei:
 * - Sobel Filter Details
 * - Apps untersucht
 * Next Steps:
 * - Corel Painter Ölfarbe anschauen
 * - Sobel Filter beenden
 * - Schauen ob man irgendwie aus der GPU schnell Daten in den RAM bekommt
 * - bestehende Software anschauen
 * 
 * 
 * 23.11.2022
 * Aufgehört bei:
 * - GPU Beschleunigung etwas genauer untersucht -> auf jeden Fall sinnvoll
 * - System neu entwerfen mit Pose als Basis
 * - ggf. virtuelle Rakel zeichnen
 * - ggf. Atelier modellieren
 * 
 * 
 * 26.11.2022
 * Aufgehört bei:
 * - Neues Projekt aufgesetzt
 * - Maske richtig anwenden mit GPU Code ist nicht einfach
 * 
 * 
 * 29.11.2022
 * Aufgehört bei:
 * - Neuen Weg für Abdrucksberechnung angefangen
 * - Rotation als nächstes
 * - GUI wieder verbinden
 * - Debug-Messages löschen
 * - TestRakel schreiben?
 * - Interpolation / RakelDrawer aktivieren
 * - Commit machen bevor ich weitermache mit Rotation?
 * 
 * 
 * 01.12.2022
 */