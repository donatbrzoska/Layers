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
 * - Positionen außerhalb der Leinwand müssen auch erlaubt sein
 * - bei geringen Auflösungen gibt es Löcher
 * - Normalen und Farben bei Canvas-Erstellung setzen
 * - Dispose Buffers auch bei neuer Canvas-Resolution
 * - Out Of Bounds anfangen zu malen fängt dann von der anderen Seite an sobald man wieder über der Leinwand ist
 * - Clip-Funktion benutzen um Pixel zu skippen
 * - 16 Bit Floats für die Farbschichten auf dem Canvas https://stackoverflow.com/questions/59728656/c-sharp-16-bit-float-conversions
 * - Tilt und Abstand zur Wand einstellbar machen
 * - "Locked"-Checkbox für Rotation
 * - Tilt in Rakel auf <90° und >0 beschränken
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
 * CMD + CTRL + LEFT/RIGHT: Springe zu vorheriger/nächster Cursorposition
 * 
 * ALT + Mouse: Spaltenweise Mutliline-Markierung
 * CTRL + ALT + Mouse: Multicursor
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
 *
 * Copy Paste Fehler, zwei mal n.x statt einmal n.x und einmal n.y
 *
 * int wird zu uint gecastet ... bei negativen Werten kommen große Zahlen heraus
 *
 * Falsche Anzahl an Threadgruppen schlägt sich heftig auf die Performance nieder (ShaderRegion falsch initialisiert)
 *
 * Integer Division liefert auch int und nicht float
 * 
 * ComputeShaderTasks: Vergessen, einen der ursprügnlichen Dispatch-Calls auszukommentieren
 *
 * Falsche API vermutet -> Man sollte nicht SetInt, SetBuffer, ... auf einem ComputeShader machen, wenn man ihn erst viel später ausführt
 *
 * Copy and Paste
 * - += mitkopiert aber eigentlich gar nicht gewünscht ...
 * 
 * Den Fehler im Shader vermutet und gedacht, ich wüsste nur irgendwas nicht. Dabei vollkommen ausgeblendet,
 * dass ich tatsächlich schon im PerlinNoiseFiller Random benutze ... (Suche nach "Random" im Projekt hätte etwas Arbeit sparen können)
 * 
 * Rotation falsch optimiert -> gedacht ich wüsste, wie die Rotationsmatrix für Rotation um z aussehen müsste
 * 
 * vermutet, dass int implizit zu uint gecastet wird bei einer Zuweisung
 * 
 * falsche 0 im Makro ersetzt -> wollte Rotation ersetzen, habe aber Position.y ersetzt ...
 * 
 * Wert setzen auskommentiert -> direkt über DebugMeldungen -> Warum ist der Wert weg?
 * 
 * Copy and Paste (beim vertex_inside Variablen umbenennen)
 * 
 * Fehler beim Abschreiben/Portieren (overlap Algorithmus, zweiten Hauptfall vermutlich einfach vergessen anzupassen)
 * 
 * Manchmal scheint es so als werden die ComputeShader einfach nicht neu geladen von Unity und dann wundere ich mich warum der Shit nicht funktioniert
 * 
 * Falsche API vermutet, InterlockedAdd darf als Value nicht direkt einen Wert aus einem 2D-Array bekommen -> Wert vorher aus Array holen
 * 
 * Falsche API vermutet, Arrays werden in HLSL nicht mit 0 initialisiert
 * 
 * Variable im Shader hieß anders als das Attribut was ich im C# Code gesetzt habe
 * -> wird vom Compiler still hingenommen aber mein Wert kommt natürlich nicht an
 * 
 * Copy and Paste (Shader Test: Execute()-Funktion kopiert und dann den Shader-Namen nicht angepasst ...)
 * 
 * Wechsel von int auf float für Volume -> vergessen die Volumenwerte bei der Normalenberechnung dann auch in floats zu speichern
 * -> 0.999999 wird zu 0 ...
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
 * Aufgehört bei:
 * - Rotation implementiert
 * - GUI wieder connected
 * - Rotation zur Mausbewegung angefangen
 *   - Übergang zwischen 360 und 0 noch schwierig
 *
 * 
 * 04.12.2022
 * Aufgehört bei:
 * - Farbe aus Reservoir holen funktioniert
 *   - Interlocked evtl. ersetzen durch zwei-Phasen Interpolation, Interlocked kann auch nur int und uint
 *   -> Inpolation über Texture2D Call abbilden?
 *
 *
 * 05.12.2022
 * Aufgehört bei:
 * - Interpolation hat Berechnung stark verlangsamt
 * - Paint ist nicht durch 16 byte teilbar -> pad einfügen zerstört das Programm
 * - StructuredBuffer Load probieren?
 * - Einfach zwei StructuredBuffer<float> und <int> verwenden statt Paint?
 *
 *
 * 06.12.2022
 * - InterlockedAdd auf TextureND wird nicht unterstützt
 * - Überhaupt in Texture schreiben und dann von anderem Shader weiterverarbeiten lassen funktioniert nicht
 * - evtl. ist Shader 1 noch gar nicht fertig? doch ist fertig in diesem Fall zumindest
 * - AppendStructuredBuffer benutzen?
 * - Texture2D für CPU -> GPU und RWTexture2D für GPU -> CPU?
 * - globallycoherent
 * - was ist der Unterschied zwischen Load und [] auf Textures?
 *
 * Aufgehört bei:
 * - ~10FPS durch Nutzung von Vektoroperationen
 * - Nächste Schritte
 *   - Volumenimplementierung + Normalmap auf Canvas
 *   - Paint Pickup
 *   - ...
 *
 *
 * 07.12.2022
 * Dispatch-Calls garantiert sequenziell ausführen:
 * - Buffer mit einem Wert beschreiben und den Buffer zurücklesen -> extrem langsam
 * - ComputeShaderTask-Queue
 *
 * aufgehört bei:
 * - Volumenimplementierung auf Canvas implementieren
 * - Rendering-Shader muss damit einhergehen
 * -> kombinieren mit Normalmap-Generierung
 *   -> evtl. Shader Region um 1 Pixel padden, damit die Normalmap exakt ist
 *   -> oder erstmal auf später verschieben, da sich an den Regions evtl. sowieso noch mal alles ändert für die weitere Optimierung
 * 
 * 
 * 12.12.2022
 * Aufgehört bei:
 * - Normalenberechnung implementiert
 * - gerade Padding implementiert aber noch nicht nicht committet
 *   - dafür wurde der Render+Normalen-Shader in zwei Teile geteilt
 * - aber auf Fehlersuche:
 *   - die Farbe wird bei wiederholtem Apply nicht in den ReservoirBuffer geschrieben -> sieht nur so aus
 *   - es scheint auch, als wenn die Perlin-Noise Funktion, egal wie scale modifiziert wird, sich nicht ändert -> sieht auch nur so aus
 *   - außerdem ist unten immer unendlich viel Farbe auf der Rakel
 *   
 * 
 * 15.12.2022
 * Es gibt auf jeden Fall Probleme mit der Interpolation
 * -> schnelle Bewegungen machen ein anderes Ergebnis als langsame
 * -> könnte eventuell daran liegen, dass bestimmte Positionen doppelt angewandt werden
 *
 * Endlich gefixt, dass unten die Farbe nicht zu Ende geht -> Bug in der XYZ Konvertierung ...
 *
 * Next Steps:
 * - Interpolator fixen (siehe 5 Zeilen ^)
 *   - außerdem: schräges Ziehen erzeugt auch Löcher, liegt vermutlich auch am Interpolator
 * - DONE UI erweitern: Clear Canvas Button, Lock Rotation Haken
 * - Es gab zuletzt auch wieder Probleme mit dem sequenziellen Ausführen der Shader (vermutlich)
 *   - auf jeden Fall kam wieder das bunte Viereck
 * - DONE Buffer auch bei neuer Canvas-Auflösung disposen
 *
 * Interpolation vermutlich nicht vollständig, weil es beim Mapping große Probleme durch das Runden gibt
 * -> 0° Rakel mit niedriger Auflösung -> reservoir_pixel ausgeben zeigt das Problem
 * -> Rotation ist auch immer noch von damals ungenau
 * 
 * aufgehört bei: EmitFromRakelShader: // TODO find out the right error
 *
 *
 * 20.12.2022
 * Es gibt verschiedene Probleme:
 * - ComputeShader Region wird zu breit berechnet
 * - Irgendwo ist ein Bug der zu Ungenauigkeiten beim Mapping von Canvas auf ReservoirPixel führt
 * - RakelInterpolator immer noch irgendwo ungenau, weil schnelles Ziehen nicht den gleichen Effekt hat, wie langsames
 *
 * TODO Interpolation am Rand
 * 
 * Obwohl die gleichen x-Positionen angewandt werden mit und ohne Interpolation gibt es einen Unterschied im Ergebnis
 * Außerdem sind die Ergebnisse generell nie exakt identisch, auch nicht bei programmatischem AddNode, ...
 * - interessant wären die Volumenwerte auf dem Canvas
 * 
 * 
 * Aufgehört bei:
 * - viele Fehler gefunden: 
 *   - Rakel darf nur so breit sein wie das Reservoir
 *   - Rakel hat doppelte Anwendungen an der selben Stelle erlaubt
 *   - Spielraum in pixel_under_rakel eingefügt
 * 
 * - RakelInterpolator Zeile 86 ist sehr misteriös
 * - pixel_in_reservoir_range macht einen großen Unterschied -> sollte es aber nicht
 * - Erste UND letzte Reihe sind immer noch leer (DebugBuffer)
 * - map_to_reservoir evtl. falsch? Siehe Grafiken
 * 
 * - InterlockedAdd funktioniert wahrscheinlich nicht
 * - UAV Counter benutzen? siehe https://www.gamedev.net/forums/topic/676919-uav-counters/ und Intel Manual
 * 
 * 
 * 14.01.2023
 * - Random war im PerlinNoiseFiller ...
 * - Mapping und Interpolation grundlegend überarbeitet
 * 
 * TODO
 * - DONE Button für y-Lock
 * - DONE Button für kontrolliertes Ziehen (Kommentar aus AddPaint extrahieren)
 * - DONE Herausfinden, warum die ShaderRegion immer ein Pixel größer ist
 * - DONE Was hat es damit auf sich, dass immer ein Teil des Reservoirs nicht geleert wird und dann später Streifen macht?
 *   - evtl. kann auch keine feste Distanz zwischen den Schritten definiert werden, weil dann bestimmte Pixel nicht getroffen werden?
 *   -> Volumen im ComputeShader dürfen beim Subtrahieren nicht kleiner als 0 werden!!!
 * - DONE DefaultConfig scheint auch irgendwo zu einem Bug zu führen, damit geht jedenfalls nichts mehr
 *   -> evtl. wird die RakelWidth irgendwo als int interpretiert
 * - Herausfinden, ob das mit der Reihenfolge der ComputeShader ein Problem ist
 * - Anwendung auf der nvidia Grafikkarte testen
 * - Wieso funktioniert das mit der Perlin-Noise Landschaft überhaupt so gut?
 * - Herausfinden, warum die 3D-Arrays immer komplett invertiert geprintet werden (upside down ist ja normal aber nicht rechts-links gespiegelt)
 * - UI Regionen noch mal ordentlich in ihre Parents verschieben
 * 
 * 
 * 15.01.2023
 * - DONE Herausfinden, warum die ShaderRegion immer ein Pixel größer ist
 * - DONE DefaultConfig scheint auch irgendwo zu einem Bug zu führen, damit geht jedenfalls nichts mehr
 * - DONE Warum ist der unterste linke Pixel 1,1?
 *   - Und wieso passiert einfach nichts, wenn man hier klickt?
 *   ->> vergessen, dass PixelKoordinaten mit 0,0 beginnen -> also einfach (1,1) abziehen in WSC
 * - Warum wird das Ergebnis manchmal beim y=0 Ziehen so streifig und manchmal nicht?
 * - Warum macht der Check auf die Mindestdistance in Rakel so einen Unterschied?
 * - DONE Der andere Weg der Interpolation (i+1) * ... scheint sogar ein richtiges Problem zu sein -> aber wieso?
 * - Wo kommt beim 30° Ziehen schon wieder der lange Streifen her???
 * - Woher kommen die senkrechten Streifen?
 *   - passieren auch beim winkligen Ziehen
 * - Wieso wird manchmal einfach an Pixeln die Rakel nicht angewandt? Pixel werden irgendwie übersprungen
 * -> kann umgangen werden mit höherer Apply-Dichte
 * 
 * TODO
 * - Interpolator noch mal anschauen
 *   - Warum macht schnelles Ziehen einen Unterschied?
 *   - richtige Anzahl der Steps finden, auch an die Begrenzung in Rakel:Apply denken
 *   - Warum macht der Check auf die Mindestdistance in Rakel so einen Unterschied?
 *   - Wieso wird manchmal einfach an Pixeln die Rakel nicht angewandt? Pixel werden irgendwie übersprungen
 *     -> kann umgangen werden mit höherer Apply-Dichte
 * - Wo kommt beim 30° Ziehen schon wieder der lange Streifen her???
 *   -> evtl. noch mal pixel_in_reservoir_range anschauen
 * - Warum wird das Ergebnis manchmal beim y=0 Ziehen so streifig und manchmal nicht?
 *   - pasiert sogar mit dem Makro und FlatFiller!!!!
 * - Woher kommen die senkrechten Streifen?
 *   - passieren auch beim winkligen Ziehen
 *   - aber vor allem zwischen nicht-interpolierten Schritten
 *      - naja nicht unbedingt
 *      - kommen sogar bei 2-Punkt Interpolation mit FlatFiller
 *   - vielleicht wird irgendwo die Konvertierung in den WorldSpace verschieden gemacht
 *      - also eine Variante ist ja in WorldSpaceCanvas und dann vielleicht noch mal im Shader suchen?
 *      -> map_to_world_space anschauen
 *      -> evtl. gibt es noch eine Stelle?
 * - irgendwo wird auch immer noch negatives Volumen erzeugt (andere Return-Condition Variante in Rakel und dann ziehen)
 * - EmitFromRakelShader:  // TODOODODODO what to do when we precisely hit a pixel?
 * - Was hat es eigentlich mit der dunklen Stelle am Anfang auf sich?
 * - Evtl. auch Padding für EmitSR?
 *   -> macht sehr komische Fehler, sollte es aber nicht
 *   
 * 16.01.2023
 * - Interpolator-Problem:
 *   - eigentlich müsste man die WorldSpace-Apply-Position auch nur auf Gridpunkten zulassen?
 *   
 *   
 * 17.01.2023
 * - Es gibt auf jeden Fall mindestens einen Bug bei der Interpolation
 * -> Abgegebene Volumes ausgeben lassen -> bei 2x5 Rakel sollte auch insgesamt nicht mehr als 2x5*Wert abgegeben werden
 * -> Volumeberechnung am Ende außerdem abhängig machen von einer OutOfRange-Rate
 *   - Aber wie berechnet sich diese OutOfRange Rate?
 *   - Wie viel Prozent der Pixel aus denen interpoliert wird, liegt insgesamt tatsächlich im Reservoir?
 * 
 * 
 * 21.01.2023
 * - Reservoir-Pixel haben komische Werte
 *   - bug in rakel_mapped?
 * - Bilineare Interpolation falsches Verfahren und auch nicht identisch zu dem was Prof. Israel vorgeschlagen hat
 *   - Neuer Ansastz aber vermutlich auch noch fehlerhaft
 * - es wäre praktisch, wenn
 *   - die Log-Ausgaben mal nicht mehr doppelt invertiert wären
 *   - ich sehen könnte, für welchen Bereich der Shader tatsächlich gelaufen ist
 *     -> CalculationPosition ausgeben?
 * - am besten mal den Canvas genau so in Keynote nachzeichnen mit allem
 *   - dann mit Debug-Ausgaben schauen, was genau passiert
 *   - und alles einzeichnen
 *   
 * - aufgehört bei:
 *   - Alles debuggen mit Keynote-Zeichnung
 *   - zuletzt: Rakel.cs //Debug.Log("calculation size=" + emitSR.CalculationSize);
 * 
 * 
 * 23.01.2023
 * - aufgehört bei: Interpolation funktioniert besser
 *   - aber es gibt noch Probleme in y-Richtung
 *   -> schauen, was jeweils die umliegenden Pixel sind, ob eins fehlt, etc. ...
 *   
 * 24.01.2023
 * - Für geringe Auflösungen funktioniert die Interpolation jetzt
 * -> warum nicht bei höheren?
 * -> der vorherige Weg für die Interpolation funktioniert einigermaßen
 * -> trotzdem gibt es das Problem, dass die Farbe bei rotierten Rakeln vermutlich nicht gleichmäßig aus dem Reservoir entnommen wird
 *    - sehr auffällig bei der overlap Interpolation
 *    - etwas weniger aber dennoch sehr auffällig bei der bilinearen Interpolation
 * -> oder auf jeden Fall mindestens nach ein paar mal auftragen nicht mehr gleichmäßig aufgetragen wird
 * -> daher kommt es dann zu eckigen Furchen im Reservoir?
 * DONE -> Aber wieso haben die Furchen im Reservoir überhaupt so eine große Auswirkung?
 * DONE   -> Genau so unlogisch wie, dass das mit dem PerlinNoise funktioniert
 * - Positionen außerhalb der Leinwand müssen auch erlaubt sein
 * - Es gibt außerdem manchmal Artefakte -> Wrden Shader evtl. nicht sequenziell ausgeführt?
 *    
 * - TODO irgendwie werden auch immer noch manchmal Pixel am Rand einfach so gesetzt
 * - TODO Streifen gibts auch immer noch!!!!!!! DONE??
 * 
 * 25.01.2023
 * 
 * ANSWERS!!!!!! SOLUTIONS!!!!!
 * - Funktionierten rotierte Rakeln nicht mit bilinearer Interpolation sogar mal? (bis auf das mit den Streifen)
 * -> Problem ist vermutlich, dass bei dem gepixelten Worldspace manche Pixel im Reservoir unterdurchschnittlich
 *    häufig geleert werden
 * -> vorher haben wir den Worldspace ja nicht gepixelt
 * -> Problem ist jetzt trotzdem noch, dass manche Bereiche erst später erreicht werden
 *    -> liegt an zu kleiner Stepsize
 *    -> denn es werden so wieder bestimmte Bereiche nicht geleert
 *    -> hier könnte die Umverteilung evtl. auch helfen
 * -> Mittelweg zwischen Umverteilung / Glättung und Stepsize finden
 * -> Worldspace muss aber vermutlich sowieso gepixelt werden um die Streifen zu vermeiden
 * -> Umverteilung quasi die einzige Lösung
 * 
 * Verschiedene Probleme und Lösungen
 * - Streifen beim Ziehen -> Worldspace pixeln und immer garantiert alle Positionen auf einer Linie verwenden
 * - Reservoir wird nicht gleichmäßig geleert
 *   - Stepsize kleiner machen um verschiedene Positionen der ins Reservoir gemappten Pixel zu ermöglichen
 *     -> bei gepixeltem Worldspace nur möglich, indem die Worldspace-Auflösung kleiner ist, als die Textur-Auflösung
 *   - Reservoir-Auflösung verringern
 *     -> daran denken, dass die im Shader auch angepasst werden muss!!!
 *     -> aber warum bleibt unten und links ein Streifen länger voll?
 *   - Multipass-Interpolation machen?
 *   - Reservoir-Volumen glätten? -> bringt leider nichts, vermutlich da ja dann immer noch schwer zu erreichende Pixel befüllt sind
 *     - evtl. ist die Glättung aber auch falsch implementiert
 *   - Threshhold für Übertragung? -> bringt nichts
 * 
 * Am besten noch mal in allen Papern nach Lösungswegen für das Problem im Allgemeinen suchen
 * - Painting Knife macht Mesh und mappt die Pixel auf dem Canvas zum nächstliegenden Vertex
 * - Baxter
 * - Industrial
 * - Detail Preserving?
 * 
 * 26.01.2023
 * - bilineare Interpolation ungeeignet für mein Problemunktioniert nicht i
 *   - denn die Reservoirpixel dürften nur zu insgesamt einem zurückrotierten Pixel beitragen
 *   -> nicht der Fall
 *      - jedes Reservoirpixel hat einen Einflussbereich von 2x2 Pixelgröße
 *      - wenn jetzt ein zurückrotiertes Pixel genau auf einem Reservoirpixel liegt,
 *      - dann hat dieses Reservoirpixel schon seinen gesamten Beitrag an das Reservoirpixel abgegeben
 *      - beeinflusst aber trotzdem noch die umliegenden zurückrotierten Pixel
 * - Lösung: Überlappung rotierter Quadrate
 * 
 * 30.01.2023
 * aufgehört bei:
 * - Sutherford Hodgman angefangen, irgendwas stimmt aber noch nicht. Macro2 zeigt bei Volumenausgaben schon ein Problem
 * 6h
 * 
 * 02.02.2023
 * - paar Fehler gefunden (vertex_inside und area calculation)
 * - nicht-rotierte Pixel funktionieren
 * - jetzt: einzelnes, rotiertes Pixel debuggen
 *   - pos_back_rotated stimmt schon mal
 * 2h
 * 
 * 03.02.2023
 * 11:30 - 13:00 1,5h
 * 14:00 - 17:15 3,25h warum verändert sich die Farbhelligkeit?
 * 20:00 - 22:15 2,25h
 * -> 7h
 * 
 * DONE Rotierte Rakeln mit Resolution 1 mit Größe >1x1 geben nicht so viel Farbe ab wie sie sollten
 * - 2x1 0° gibt sogar mehr Farbe ab -> unten links werden 42 mehr rausgeholt -> Lösung: Array mit 0 initialisieren
 * DONE Gleiches gilt für rotierte und nicht-rotierte Rakeln mit höheren Auflösungen
 * 
 * Abgeben sollte eine Rakel insgesamt immer Anzahl Pixel x 1000
 * 
 * DONE 1x1 Rakel sollte bei Auflösung 2 eigentlich auch 4000 abgeben oder?
 * 
 * DONE negative Volumenwerte im Reservoir verhindern, ist das so richtig?:
 *    volume_to_emit[y+1][x+1] = min(target_volume, available.volume);
 *    nein war nicht richtig
 * 
 * aufgehört bei:
 * - Prüfen: Farbabgabe sollte sich nach einem Klick in allen Variationen von Rotation, Abmessungen und Auflösung verbrauchen
 * 
 * 04.02.2023
 * 20:45 - 1:00
 * -> 4,25h
 * - Algorithmus führt zu schlechter Laufzeit
 *   - compute_intersection könnte man evtl. noch optimieren
 * - 45° funktioniert - Ich musste noch mal richtig verstehen, was reservoir_pixel eigentlich für eine Bedeutung hat und wie man das berechnet
 * - 30° macht aber noch Probleme
 *   - liegt evtl. an den Volumen Ints oder an der Restentnahme -> negative Volumenwerte im Reservoir möglich?
 *   - Resolution 2 nimmt sogar mehr Farbe aus dem Reservoir als drin ist
 * - Scheint auch so als wenn wir besser auf Floats statt Ints umsteigen sollten für die Volumenwerte
 * >>> Erstmal ~ halb so schlimm, musste really_available_volume doch mit overlap berechnen
 * 
 * Debughilfe für calculate_overlap_:
 *   bool debug_this = adjacent_reservoir_pixel.x == 0 && adjacent_reservoir_pixel.y == 0
 *                  && f2_eq(reservoir_pixel, float2(0.0, 0.5));
 *                  
 * DONE compute_intersection optimieren
 * - ist vermutlich gar nicht das Problem, eher die vielen Schritte des Algorithmus
 * DONE Ternärer Operator möglicherweise auch ineffizient?
 * TODO Tilted
 * LATER senkrechte Streifen sind wieder da
 * DONE Winkel != 45° sehen teils noch mit hohen Auflösungen komisch aus
 *      - könnte an Resten durch Integers liegen, weil dann krummere Werte rauskommen
 * DONE Teils erwischt man bei einer leeren Rakel mit einem anderem Winkel doch noch bisschen Farbe, könnte an dem Rest 1-2 liegen
 *      - könnte an Resten durch Integers liegen
 *      
 * TODO Kreisschnittfläche
 * DONE Bilinear und nearest neighbour zurückbringen für Benchmarks
 * 
 * DONE Farbmenge durch Config konfigurierbar
 * DONE Positionen außerhalb
 * 
 * MAYBE LATER Benchmark: float vs integer addition
 * 
 * 06.02.2023
 * 15:30 - 18:45 3.25h
 * 19:15 - 22:00 2.75h
 * -> 6h
 * aufgehört bei:
 * - ReservoirDuplication-Shader: durch Integer-Rundung verbleibende Farbreste aus Rakel löschen
 * 
 * 
 * 10.02.2023
 * 16:30 - 17:00
 * - Mapping bei tilted Rakeln wahrscheinlich extrem unperformant
 * 
 * 12.02.2023
 * 13:30 - 15:15 1,75h
 * 17:30 - 19:45 2,25h
 * 00:00 - 1:45 1,75h
 * 5,75h
 * 
 * DONE RotationLocked Property wird am Anfang nicht richtig angezeigt
 * DONE Simple Overlap funktioniert nur bei Resolution 1
 * 
 * TODO Architektur überdenken
 * DONE RakelResolution in die GUI
 * DONE GUI Elemente müssen Apply blockieren
 * MAYBE LATER Jitter in die Rotation einbauen?
 * TODO Perlin Noise auf übertragenem Volumen wäre interessant
 * - Shader Model > 1 unterstützt kein noise(), also selbst implementieren
 * - https://fancyfennec.medium.com/perlin-noise-and-untiy-compute-shaders-f00736a002a4
 * - https://github.com/z4gon/cg-perlin-noise-shader-unity
 * 
 * Testplan:
 * - niedrige, hohe Textur-Auflösung
 * - Rotation fest, Rotation wackelig
 * - Smoothing mit verschiedenen Kernelgrößen, Discard und Reservoir-Auflösung
 * 
 * 13.02.2023
 * 13:15 - 19:30 6,25h
 * 22:00 - 00:00 2h
 * 8,25h
 * - Refactoring-Tag
 * - offen:
 *   - IntelGPUShaderRegion
 *   - Anchor in Config setzbar machen? Muss aber auf Enums basieren, weil Width und Length unbedingt in der Rakel korrigiert werden müssen
 * 
 * 17.02.2023
 * 12:45 - 14:00 1,25h
 * 15:15 - 16:30 1,25h
 * 2,5h
 * - CanvasReservoir müsste dupliziert werden für Emit von dort
 * -> spätestens mit mehreren Farbschichten extrem Speicheraufwändig
 * 
 * - Kann man einen generischen Shader schreiben für das Emit aus Canvas- und Rakel-Reservoir?
 * 
 * Plan für dieses Semester:
 * - bidirektionaler Farbaustausch
 *   - Reservoir-Duplication <-> Doppelt Sampeln
 *   - Doppelt Sampeln: Integer vs Float
 * - tilted Rakeln wären echt gut
 * 
 * 19.02.2023
 * 17:15 - 18:00 0,75h
 * 20:00 - 21:00 1h
 * 22:00 - 23:15 1,25h
 * 3h
 * 
 * - Interpolation ins PickupReservoir sicht nicht 100% sauber aus
 * - 3D-Effekt verschwunden
 *   -> vermutlich weil ja auch wieder Farbe mitgenommen wird
 * - CopyBufferToBufferShader macht die Farben komplett Schwarz
 *   - evtl. macht es doch Sinn, CopyBufferToBuffer aufzuteilen
 *   
 * 20.02.2023
 * 14:15 - 14:45 0.5h
 * 15:45 - 16:00 0.25h
 * 18:30 - 21:30 3h
 * 3,75h
 * 
 * DONE Enable Debug in ComputeShaderTask kann auch einfach ein bool sein
 * -> Buffergröße ist sowieso immer gleich der ShaderRegion-Größe
 * DONE Klammern für Funktionen in den Shadern vereinheitlichen
 * 
 * Dadurch dass die Farbe sich beim Übertragen auf den Canvas im Pixel verteilt,
 * kann man nicht davon ausgehen, dass beim abnehmen an der selben Stelle wieder
 * genau gleich viel Farbe mitgenommen wird. Denn die Farbe ist sozusagen über den
 * Rand der Rakel hinaus auf der Leinwand.
 * 
 * - aufgehört bei:
 *   - ApplyCanvasInputBuffer debuggen -> immer noch herausfinden wieso da alles schwarz wird
 * - Zwischenbuffer evtl. entfernen?
 * 
 * 22.02.2023
 * 11:00 - 11:45 0,75h
 * 12:15 - 12:30 0,25h
 * 13:30 - 17:00 3,5h
 * 4,5h
 *
 * - aufgehört bei:
 *   - Zwischenstand für bidirektionalen Farbaustausch erstmal kopiert und neu angefangen
 *   - Nutzung von include
 *   - nächster Schritt: Interpolationen an der Basis vereinheitlichen wäre riiichtig gut
 *      - Radius neben ReservoirPixelRounded angeben?
 *   - Unterschiedliche Auflösungen auch für bilinear und polygon clipping zu implementieren
 *     wird anstrengend und ist vermutlich nicht sinnvoll
 *     - wäre ansonsten sehr schwierig mit dem Vereinheitlichen der Basis von bilinear und
 *       polygon clipping
 *
 * 23.02.2023
 * 16:45 - 19:45 3h
 * 20:00 - 21:45 2,75h
 * 5,75h
 * 
 * DONE RakelApplicationReservoir -> ApplicationReservoir
 * DONE if (x.volume > 0 ) irgendwie ersetzen
 * - Grundproblem dahinter: Verhindern, dass leere Farbe als schwarze Farbe interpretiert wird
 * 
 * DONE Parameter Einrückungen in Rakel.cs konsistent machen
 * DONE RakelEmittedPaint -> rakelEmittedPaint
 * DONE newline unter map_to_world_space in EmitFromRakelShader
 * DONE double declaration warning
 * 
 * aufgehört bei:
 * - EmitFromCanvas wieder angefangen
 * - irgendein Buffer ist noch null
 * 
 * 24.02.2023
 * 13:30 - 14:15 0,75h
 * 14:30 - 16:30 2h
 * 17:15 - 21:15 4h
 * 7h
 * 
 * TODO WorldSpaceCanvas umbennen / mit OilPaintCanvas vereinen?
 * DONE remove parameter oilPaintCanvas.Reservoir.Buffer);
 * DONE remove ReservoirFiller folder
 * DONE ShaderTools -> ComputeShaderTools
 * LATER ApplyBufferToRakel: Kommentar anpassen, CalculationPosition kann raus?
 *      // calculate pixel position on canvas
        int2 rakel_pixel = id.xy + CalculationPosition;
 * 
 * aufgehört bei:
 * - irgendwie wird schwarze Farbe erzeugt, wenn welche aufgenommen und wieder abgegeben wird
 * 
 * 27.02.2023
 * 13:15 - 13:45 0,5h
 * 14:00 - 16:15 2,25h
 * 16:30 - 16:45 0.25h
 * 17:15 - 19:00 1,75h
 * 19:45 - 21:45 2h
 * 6,75h
 * 
 * Problem mit Farbverdunklung erstmal gefunden
 *- neue Probleme:
 *   - DONE Wenn einmal Farbe aufgetragen ist, wird sie zwar aufgenommen
 *     aber nicht gelöscht (Oberfläche/Schatten/Normalmap ändern sich kein bisschen)
 *     -> Problem gab es nicht wirklich, also es gab einen Bug aber nicht so signifikant wie erwartet
 *   - DONE wenn ich genau so viel Farbe auftrage wie ich lösche, dann reicht die Farbe ewig
 *     - es wird generell auch immer 3x so viel Farbe aufgetragen wie am Anfang in der Rakel ist
 *     -> das liegt teilweise am PerlinNoise
 *     -> aber kommt auch beim FlatFiller vor, da ist es 2x so viel Farbe
 *     -> es ist nicht zwei mal so viel Farbe, die Summenbildung geht über z=0 und z=1
 *     >> Da ist einfach viel Farbe im ApplicationReservoir die immer wieder abgegeben und
 *        aufgenommen wird
 *   - DONE wenn man in aufgetragene Farbe reinwischt, kommt dort am Rand manchmal eine
 *     Farbe die man nicht erwarten würde
 *   - DONE bei rotierten Rakeln kommt es bei der Farbaufnahme zur Bildung von grellen Farbflecken
 *   
 * DONE Farbabgabe konfigurierbar machen
 * DONE Clear Canvas <-> Clear Rakel Button
 * TODO Farbmischung bei Interpolation muss später auch eine mix-Funktion benutzen
 * 
 * 28.02.2023
 * 11:00 - 12:15 1,25h
 * 15:30 - 16:00 0.5h
 * 17:00 - 18:15 1,25h
 * 
 * DONE Alpha für geringe Volumen
 * TODO Debug-Werte über Inspector setzen
 * DONE EMPTY_PAINT_COLOR -> CANVAS_COLOR
 * LATER immer noch Streifen
 * -> vermutlich z.b. wegen Unterschied in y-Richtung aber nicht in x-Richtung
 * 
 * Vergleich mit CPU-Performance: Messungen mit dem Benchmark-Flag schwierig
 * -> Beim realen Verwenden bemerkt man einen Unterschied, an den FPS nicht wirklich
 * 
 * 19:15 - 20:45 1,5
 * 
 * 4,5h
 * 
 * aufgehört bei:
 * - bidirektionaler Farbaustausch mit alpha für geringe Volumen funktioniert
 * - TODO tilted
 * - Paper angeschaut
 * - bisherige Tools angeschaut
 * 
 * 13.03.2023
 * 11:45 - 12:30
 * 
 * 13:00 - ?
 * 
 * 
 * 20.03.2023
 * Double Buffering macht sogar einen 25-40% Performance-Boost
 * - aber macht auch alles kaputt
 * DONE Smoothing ist kaputt, irgendwo entsteht schwarze Farbe
 * - vermutlich wenn nur sehr wenig Farbe von der Leinwand mitgenommen wird?
 * TODO manchmal kann man einfach nicht malen nach dem Apply
 * - irgendwas beim Double Buffering übersehen?
 * DONE Rakel mit Länge 10 und PerlinColored hat alles schwarz gemacht
 * - neue Rakel Rakel und Leindwand hat nicht mehr geholfen
 * - nur Unity Neustart hilft
 * - passiert, wenn ein Teil der Rakel OOB ist, aber nicht bei kleinen Rakeln
 * - evtl. Speicher voll? aber bei Auflösung 20? ...
 * DONE Nearest Neighbour ist auch kaputt
 * - vielleicht liegts auch nur am Rendering?
 * - Problem besteht nur bei rotierter Rakel
 */
