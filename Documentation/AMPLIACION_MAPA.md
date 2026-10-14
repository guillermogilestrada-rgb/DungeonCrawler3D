# Ampliación del mapa (blocking)

Escena: `Assets/Scenes/Dungeon_FrozenMine.unity`. Todo está montado con los prefabs de `Assets/Prefabs/FrozenMine` (suelo, techo, pared, escalera, raíl, vagoneta, barricada, farol, etc.). No hay arte nuevo.

## Qué se añadió (43 celdas de 4 × 4 m)

**Ala este** (sale del Almacén por la pared este de la fila sur, abierta con un pórtico)
- `09_RampaEste`: bajada de 3 rampas (−4,5 m). **Vagoneta_BajadaEste** arranca arriba, acelera por la pendiente y rompe `Cajas_BloqueoSalaBaja` a mitad de la rampa.
- `10_SalaBaja`: sala de 3 × 3 celdas a −4,5 m con pilares, cristales y cajas. **Vagoneta_SalaBaja** rompe `Cajas_BloqueoTunelCristal`.
- `11_TunelCristal`: túnel y tres tramos de escalera (−4,5 → 0 m).
- `12_BalconCristal`: sala final de 3 × 2 celdas a nivel 0.

**Ala oeste** (sale de la Cámara de hielo por la pared oeste, abierta con un pórtico)
- `13_GaleriaOeste`: galería con dos escaleras que suben a +3 m.
- `14_TerrazaCristal`: terraza elevada a +3 m. **Vagoneta_Terraza** rompe `Cajas_BloqueoPasoNorte`.
- `15_PasoNorte`: tramo plano y bajada de 3 rampas (3 → −1,5 m). **Vagoneta_BajadaNorte** rompe `Cajas_BloqueoCamaraNorte`.
- `16_CamaraNorte`: cámara final a −1,5 m.

Los objetos de decoración y raíles están en `ENVIRONMENT/17_Decoracion_Ampliacion`, los faroles en `LIGHTING`, las luces frías en `ColdLights` y las vagonetas y barricadas en `GAMEPLAY`.

## Cambios que afectan a lo que ya existía
- Se quitaron dos paredes para abrir las alas: `Wall_3_6_1_0` (Almacén, fila sur) y `Wall_-4_5_-1_0` (Cámara de hielo).
- `Minecart.cs`: campo nuevo `downhillBoost` (por defecto 0, no cambia las vagonetas existentes). Las dos vagonetas de bajada lo usan con valor 6.
- Las rampas son `PF_Floor4` y `PF_Ceiling4` inclinados (≈20,6°, dentro del límite de 50° del jugador). Los dinteles `Dintel_*` son `PF_Wall4` escaladas que cierran los saltos de techo.

## Qué probar en Unity
1. Abrir la escena y revisar la Console.
2. Recorrer las dos alas: rampas, escaleras, techos y paredes sin huecos.
3. Activar cada vagoneta con E: que rompa su barricada y quede aparcada sin bloquear el paso.
4. Pulsar R: debe volver a la entrada y reponer todas las vagonetas y cajas.
5. Si una vagoneta de bajada va demasiado rápida o lenta, ajustar `Downhill Boost` en su Inspector.
