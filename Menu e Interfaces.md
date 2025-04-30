# 🎮 Menús e Interfaces - Proyecto DOOM Unity

En este documento se recoge y resume toda la funcionalidad e implementación de las **escenas relacionadas con los Menús e Interfaces** del proyecto.

> [!NOTE]
> Esta documentación solo cubre **la parte del sistema de menús e interfaces**. El resto de elementos del juego (enemigos, armas, IA, etc.) no se recogen aqui.

---

## 🧭 ESCENA: **MainMenu**

### 📝 RESUMEN DE ESCENA
Esta escena corresponde al **menú principal del juego**. Desde aquí, el jugador puede:

- Iniciar una nueva partida
- Ajustar opciones
- Ver los créditos
- Salir del juego

### 📜 SCRIPTS

- **MenuActions.cs**  
  Controla toda la lógica de los botones: qué debe activarse según el botón pulsado.  
  Acciones implementadas:

  - **Un Jugador**: De forma temporal, salta a una escena de trabajo llamada `LoadScene`.
  - **Multijugador**: ❌ **(NO IMPLEMENTADO)**
  - **Opciones**: Abre un menú con configuraciones como:
    - Audio
    - Sensibilidad  
  - **Créditos**: Salta a la escena de créditos ✅ **(IMPLEMENTADO)**
  - **Salir**: Cierra el juego ✅ **(IMPLEMENTADO)**

- **MenuSelector.cs**  
  Controla el movimiento de la calavera sobre los botones, el cambio de color al seleccionar y accede a la opción al pulsar `Enter`.

- **SceneTransition.cs**  
  Se encarga de la **pantalla en negro** para suavizar la transición entre escenas.

- **ScreenEffectManager.cs**  
  Controla los **efectos de vibración de pantalla**.

- **ScreenMessageControl.cs**  
  Muestra mensajes contextuales en pantalla dependiendo del botón sobre el que se sitúe el selector.

---

## ⚙️ ESCENA: **LoadScene**

### 📝 RESUMEN DE ESCENA
Escena de trabajo. El nombre actual no representa su función final. Actualmente se ha implementado:

#### 🔄 CARGA DE ESCENA FALSA

Carga simulada que funciona con el script:

- **FakeLoaderScene.cs**  
  Recibe como parámetros:
  - Tiempo de carga deseado (en segundos)
  - Imágenes que deben mostrarse secuencialmente

> [!CAUTION]
> Esta carga no representa una carga real de datos. Solo simula un proceso de espera.

---

### ⏸️ MENÚ DE PARTIDA

Al pulsar la tecla `ESC` aparece un pequeño menú con las siguientes opciones:

- **Reanudar**: Quita el menú ✅ **(IMPLEMENTADO)**
- **Reaparecer**: Mueres en la partida y reapareces ❌ **(NO IMPLEMENTADO)**
- **Salir**: Abre un submenú de confirmación para salir de la partida ✅ **(IMPLEMENTADO)**

**Script usado**:

- **OptionActions.cs**  
  Maneja toda la lógica de los botones del menú de pausa.

> [!WARNING]
> El botón de *Reaparecer* aún no tiene lógica implementada. No recomiendo su ejecución actualmente, ya que puede generar errores si se pulsa reiteradamente.

---

### 🧑‍🚀 INTERFAZ DE USUARIO

Implementación temprana de la **UI de partida**. Simula un casco. Como prueba:

- Al pulsar las teclas de dirección `↑` y `↓`, la vida sube y baja.
- Conforme cambia la vida:
  - La interfaz se ve más dañada.
  - La **cara del marine cambia** si la vida baja o si se pulsa `A` o `D`.

**Scripts usados**:

- **CabezaDoom.cs**  
  Cambia el sprite de la cabeza según la acción del jugador y el nivel de vida.

- **BarraProgresoDoom.cs**  
  Actualiza la barra de vida e interfaz. Permite modificar la vida con las teclas de dirección.

> [!NOTE]
> la funcionalidad dada por `↑` y `↓` es provisional y se usa para pruebas visuales. No representa el comportamiento final del HUD.

---
