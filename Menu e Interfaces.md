# 🎮 Menús e Interfaces - Proyecto DOOM Unity

En este documento se recoge y resume toda la funcionalidad e implementación de las **escenas relacionadas con los Menús e Interfaces** del proyecto.

> [!NOTE]
> Esta documentación solo cubre **la parte del sistema de menús e interfaces**. El resto de elementos del juego (enemigos, armas, IA, etc.) no se recogen aquí.

---

## 🧭 ESCENA: **MainMenu**

### 📝 RESUMEN  
Esta escena representa el **menú principal del juego**. Desde aquí, el jugador puede navegar a distintas secciones o cerrar el juego.

---

### 🎮 MENÚ PRINCIPAL

| Botón         | Funcionalidad                                                                 |
|---------------|-------------------------------------------------------------------------------|
| Un Jugador    | Carga la escena de prueba `LoadScene`.                                       |
| Multijugador  | ❌ *No implementado*.                                                         |
| Opciones      | Abre el submenú con configuraciones de audio y sensibilidad.                 |
| Créditos      | Cambia a la escena de créditos. ✅ *Implementado*.                           |
| Salir         | Cierra el juego. ✅ *Implementado*.                                          |

---

### ⚙️ SCRIPTS USADOS EN **MainMenu**

- **MenuActions.cs**  
  Controla las acciones según el botón pulsado.

- **MenuSelector.cs**  
  Mueve la calavera por los botones, gestiona colores y entrada con `Enter`.

- **SceneTransition.cs**  
  Implementa la pantalla negra de transición entre escenas.

- **ScreenEffectManager.cs**  
  Aplica efectos de vibración visual.

- **ScreenMessageControl.cs**  
  Muestra mensajes contextuales en pantalla al pasar por un botón.

- **SettingsManager.cs**
  Controla las variables globales para ajustar las opciones

- **SettingsUI.cs**
  Enlaza las opciones con sus correspondiente Sliders.

- **AudioButton.cs**
  Script que reproduce sonidos.

---

## ⚙️ ESCENA: **LoadScene**

### 📝 RESUMEN  
Escena temporal de pruebas. Simula una carga de escena y contiene la interfaz del HUD y el menú de pausa.

---

### 🔄 CARGA DE ESCENA FALSA

Simulación de carga visual. No representa una carga real.

| Parámetro              | Función                                                             |
|------------------------|---------------------------------------------------------------------|
| Tiempo de carga        | Define la duración de la carga simulada (en segundos).             |
| Lista de imágenes      | Se muestran de forma secuencial durante la carga.                  |

- **Script**: `FakeLoaderScene.cs`

> [!CAUTION]
> Solo simula el proceso de carga; no hay carga real de datos.

---

### ⏸️ MENÚ DE PAUSA

Se accede pulsando `ESC` durante la partida.

| Botón        | Funcionalidad                                                                 |
|--------------|-------------------------------------------------------------------------------|
| Reanudar     | Cierra el menú de pausa y continúa el juego. ✅ *Implementado*.               |
| Reaparecer   | Mata y reaparece al jugador. ❌ *No implementado*.                            |
| Salir        | Abre un submenú de confirmación para salir de la partida. ✅ *Implementado*.  |

- **Script**: `OptionActions.cs`

> [!WARNING]
> El botón *Reaparecer* no tiene lógica implementada. Su uso puede causar errores.

---

### 🧑‍🚀 HUD / INTERFAZ DE JUEGO

Primer prototipo de HUD simulando el casco del jugador.

**Acciones temporales de prueba:**

| Tecla         | Efecto en HUD                                                               |
|---------------|------------------------------------------------------------------------------|
| `↑` / `↓`     | Sube / baja la vida (para pruebas visuales).                                |
| `A` / `D`     | Cambia el sprite de la cabeza del marine.                                   |

**Cambios visuales:**

- Vida baja = interfaz más dañada
- Cambios en el sprite del rostro del jugador según salud y acciones

#### SCRIPTS USADOS

- **CabezaDoom.cs**  
  Cambia el sprite de la cara del jugador según el estado.

- **BarraProgresoDoom.cs**  
  Controla la barra de vida y su comportamiento frente a inputs.

> [!NOTE]
> Las funcionalidades actuales son provisionales, pensadas solo para pruebas del HUD.
