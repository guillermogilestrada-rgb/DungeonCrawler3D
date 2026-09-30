# Historial de esta intervención

Este paquete no contenía el repositorio Git original del alumno. Se ha creado un historial local separado, con autoría de OpenAI Codex y commits realizados durante la intervención. No se han inventado commits de días anteriores.

## Inspeccionar sin afectar a tu repositorio

Desde la carpeta que contiene `DungeonCrawler`:

```sh
git clone DungeonCrawler/Documentation/Historial_implementacion.bundle RevisionMina
git -C RevisionMina log --oneline
```

Esto crea una copia independiente con el historial incluido. Unity Hub puede abrir esa copia porque contiene Assets, Packages y ProjectSettings.

## Si ya tienes repositorio

La opción sencilla es copiar los cambios al proyecto conservando tu `.git`, revisar `git diff` y registrar una integración que describa honestamente el cambio. No borres los commits anteriores ni hagas pasar un bloque ya terminado por una evolución que no ocurrió.

Si prefieres conservar los commits de implementación:

1. Guarda o confirma tus cambios locales antes de integrar.
2. Desde tu repositorio, crea una rama de integración.
3. Importa la rama del bundle (ajusta la ruta al archivo):

```sh
git switch -c integrar-mina-helada
git fetch /ruta/Historial_implementacion.bundle refs/heads/master:refs/remotes/mina-entrega/master
git log --reverse --oneline refs/remotes/mina-entrega/master
```

4. El primer commit es la base recibida: **no lo apliques**. Aplica con `git cherry-pick HASH` los commits posteriores, en el orden que muestra el log. Sus hashes exactos están en `COMMITS.txt`. Revisa cualquier conflicto contra tus cambios actuales.
5. Abre y prueba la escena. Después integra esa rama como hagas habitualmente en tu proyecto.

## Si no tenías repositorio

Usa la copia clonada del bundle como punto de partida. Crea un repositorio vacío en tu cuenta y configura su URL real:

```sh
git remote remove origin
git remote add origin URL_REAL_DE_TU_REPOSITORIO
git branch -M main
git push -u origin main
```

No se ha creado ningún repositorio remoto desde aquí. El enlace de entrega será el de tu repositorio una vez publicado.

Continúa con commits reales, por ejemplo `Ajustada sensibilidad tras prueba`, `Corregido collider de...` o `Añadidas capturas de entrega`, cuando hagas esas tareas.
