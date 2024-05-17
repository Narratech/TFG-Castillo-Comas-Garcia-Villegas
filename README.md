# TFG-Castillo-Comas-Garcia-Villegas
<h1 align="center"> Procedural OVO Worlds </h1>
<p align="center"><img src=https://github.com/Narratech/TFG-Castillo-Comas-Garcia-Villegas/assets/82498887/9df05af0-6396-4a3b-b2cb-0b34eca57165
/></p> 

## Tabla de contenidos:
---

- [Badges](#badges)
- [Descripción y contexto](#descripción-y-contexto)
- [Guía de instalación](#guía-de-instalación)
- [Guía de usuario](#guía-de-usuario)
- [Código de conducta](#código-de-conducta)
- [Autor/es](#autores)
- [Limitación de responsabilidades - Solo BID](#limitación-de-responsabilidades)

## Badges
--- 
- Movimiento y camara de ejemplo para moverse por el mapa: [![Static Badge](https://img.shields.io/badge/Unity-Version?style=for-the-badge&label=Modular%20First%20Person%20Controller)
](https://assetstore.unity.com/packages/3d/characters/modular-first-person-controller-189884)
- Unity: ![Static Badge](https://img.shields.io/badge/Unity-Version?style=for-the-badge&logo=Unity&label=2022.3.1f1%20LTS)
- PONER DE DONDE HEMOS SACADO LOS ARBOLE/ PLANTAS Y ANIMALES

### Badges que solicitamos:
---
Para el mejor y correcto uso de la herramienta solicitamos a los equipos que sumen sus herramientas al catálogo  de sumar el badge por el uso del microservicio de Procedural OVO Worlds.

**PONER LINKASO A NUESTRO REPO DE DESCARGA, ASSET STORE LO K SEA**

![Static Badge](https://img.shields.io/badge/passed-version?style=for-the-badge&label=Procedural%20OVO%20Worlds%201.0.1)

## Descripción y contexto
---
Procedural OVO Worlds es una herramienta de creacion de mapas procedurales 3D, es una potente solucion que permite generar sus propios mapas personalizados con una gran variedad de estilos y elementos.
Principalmente la herramienta proporciona dos estilos principales: **Cartoon y Cúbico**(tipo Minecraft), proporcionando asi opciones visuales diversificadas para satisfacer las preferencias de los usuarios.

En cuanto a la generacion y estilos de terrenos,esta es capaz de generar una amplia gama de estilos, permitiendo asi adaptarse a multiples/diversas tematicas y necesidades creativas del usuario. Los tipos de terreno que genera son biomas y estos determinan las caracteristicas de este tipo de terreno.
Por otro lado la herramienta dispone de dos sistemas de generacion de objectos, el sistema de generacion de puntos de interes el cual sirve para genrar objectos que se les quiere dar una mayor importancia y por otro lado el 
sistema de folliage para poblar el mapa generado con estos objectos.

## Guía de instalación
---
**Requisitos Previos:**
  - Tener instalado Unity cuya version sea superior a 2022.3.1f1 LTS.
  - Tener descargado el paquete Procedural OVO Worlds.
    
Pasos a Seguir:

**1. Abrir el Proyecto en Unity:**
- Iniciar Unity Hub.
- Selecciona el proyecto en el que deseas importar el paquete o crea uno nuevo si es necesario.

**2. Importar el Paquete:**
- En la interfaz de Unity, ve al menu "Assets".
- Selecciona "Import Package" > "Custom Package".
- Navega hasta la ubicacion donde descargaste el paquete.
- Selecciona el paquete y haz clic en "Abrir".
   
**3. Confirmar los Elementos a Importar:**
- Aparecera una ventana que muestra todos los elementos que se importaran del paquete.
- Puedes desmarcar elementos que no desees importar en este momento.
  
**4. Importar los Elementos:**
- Haz clic en "Import" para comenzar a importar los elementos seleccionados.
- Espera a que Unity complete el proceso de importacion.
  
**5. Verificar la Importacion:**
- Una vez completada la importación, verifica que todos los elementos se hayan importado correctamente.

## Guía de usuario
---

En primer luegar encontramos el prefab **Map Generator**, ene ste podemos encontrar el **componente principal** de la herrmienta que es el ***Map Generator***.

![image](https://github.com/Narratech/TFG-Castillo-Comas-Garcia-Villegas/assets/82498887/cb439518-f64e-4b25-8982-0566de622268)

*Basics Elements*:
- **DrawMode**: Indica el tipo de mapa que se va agenerar, este puede ser
    - NoiseMap: mapa 2D en blanco y negro, que representa el ruido de perlin
    - ColorMap: mapa 2D a color
    - CubicMap: mapa 3D al estilo minecraft
    - CartoonMap: mapa 3D al estilo cartoon
- **GameObject Map3D**: es el gameobject padre en el cual se va a generar todo el mapa, incluyendo objectos.
- **MapSize**: Tamaño del mapa que se va a generar
- **Seed**: Este atributo es un entero que se utiliza para **inicializar el generador de números aleatorios utilizado por Unity para la creación del mapa ruido de distribucion de los biomas por el mapa**. Al definir este valor, se especifica el valor inicial de la semilla del generador de números aleatorios, lo que, a su vez, afecta visualmente a la apariencia del ruido creado en la escena. Definiendo de forma diferente este valor, se pueden obtener una variedad de vistas del ruido creado.
- **Offset**: La propiedad 'offset' representa el desplazamiento del ruido generado, este desplazamiento permite ajustar la posición inicial del ruido en la escena.Este es un vector2 que especifica la cantidad de desplazamiento en las direcciones horizontal y vertical.

*Biomes*:

BiomeGenerator es el encargado de la distribucion de forma dinamica de los biomas por el mapa utilizando un mapa de ruido cuya semilla es la que se ha establecido enteriormente. En este encontramos:
- **Noise Size**: es el tamaño del ruido, es decir controla la escala o tamaño de las "caracteristicas" generadas por el ruido de perlin en el espacio, a mayor tamaño menos cambios bruscos habra en el terreno y a mas quequeño habra mas detalles mas finos y cambios de terreno mas bruscos.
- **Influence Range**:
- **Biomes**:  Conjunto de biomas que van a conformar el mapa
  
*Curves*:
- **Noise Transition**:
- **Height Transition**:

*Interest Points*
- **Interest Points**:

*Boolean Options*
- **IsIsland**: Booleano que permita que el mapa generado tenga forma de isla
- **Auto Update**: Booleando que permite que cualquier cambio realizado en este componete se actualize directamente creando un mapa nueco intsantaneo
- **Generate Objects**: Booleando que permite la creacion de gameobjects en el mapa
- **Generate InterestPoints**: Booleando que permite la creacion de los puntos de interes anteriormente establecidos en el mapa.

#### ***Biome Object***

Este objecto determina el comportamiento que va atener el terreno de su bioma, para ello, dicho comportamiento se establece con los parametros de: 

![image](https://github.com/Narratech/TFG-Castillo-Comas-Garcia-Villegas/assets/82498887/ff95e710-a7a1-4873-bdb0-0f4a95e8feda)

*Noise Settings*

- **Noise Scale**:  El factor de escala del ruido generado.Un valor mayor producirá un ruido con detalles más finos
- **Octaves**: El número de octavas utilizadas en el algoritmo de ruido.Cada octava es una capa de ruido que se suma al resultado final.
   A medida que se agregan más octavas, el ruido generado se vuelve más detallado.
- **Persistance**: La persistencia controla la amplitud de cada octava.Un valor más bajo reducirá el efecto de las octavas posteriores de las octavas posteriores
- **Lacunarity**: Un multiplicador que determina qué tan rápido aumenta la frecuencia para cada octava sucesiva en una función de ruido de Perlin
- **Offset**: Desplazamiento del ruido generado

*Biome Generation*

- **Weight**:
- **Density**: Numero que representa la densidad del bioma en el mapa

*Terrain Transformation*

- **Max Height**: Altura maxima del terreno que puede llegar a generar el bioma
- **Min Height**: Altura minima del terreno que puede llegar a generar el bioma

*Foliage Settings*

- **Foliages**: Conjunto de objectos que se pueden generar en el bioma

*Test values*

- **Color**: ?¿
  
#### **Foliage Object**

Este objecto, perimite la instaciacion de objectos a lo largo del mapa generado.
![image](https://github.com/Narratech/TFG-Castillo-Comas-Garcia-Villegas/assets/82498887/51f04c54-ac98-4bfe-b3aa-ebc6be2ceff7)


*Prefab Properties*

- **Prefab**: Objecto que se quiere instanciar
- **Foliage**: Boleando que indiqca si el objecto que queremos instanciar no tenga espacio libre alrededor de este, un ejemploclaro del uso del mismo es la hierba ya que nos da igual que haya hierba alrededor de hierba
- **Unit Space Separation**: Unidades de unity de separacion que requiere ese objecto con respecto a otros("con el YES")
  
*Density Features*

- **Density Curve**: Curva que permite establecer de que parte a que parte hay mas probabilidades de que se genere el objecto
- **Density**: Densidad del objecto que queremos instanciar
- **Noise Scale**: 

*Transform Properties*

- Rotation
  - **Random Rotation**: Si el objecto va a tener rotacion Random
  - **Min Rotation**: Rotacion minima del objecto
  - **Max Rotation**: Rotacion maxima del objecto
  - **Rotation**: Si el objecto no va a tener rotaciones random, que rotacion basica va a tener el objecto
- Scale
  - **Random Scale**: Si el objecto va a tener escala Random
  - **Min Scale**: Escala minima del objecto
  - **Max Scale**: Escala maxima del objecto
  - **Scale**: Si el objecto no va a tener escala random, que escala basica va a tener el objecto
- Height
  - **Random Height**: Si el ojecto va a tener una altura random
  - **Min/Max Height**: Altura minima y maxima del objecto

*Advanced Settings*

- **Enviroment Rotation**: Si queremos que el objecto se adapte al terreno, es decir posicionarse correctamente con respecto a este
- **Subsistence in the ground**: Cantidad del objecto que se puede hundir en el suelo

#### **TextureUpdater**

La malla de terreno utiliza un material que utiliza nuestro shader personalizado
Este componente se encarga de actualizar y modificar los parametros de dicho material.
El componente obtiene toda esta informacion de un ScriptableObject de tipo TextureData, dentro de este scriptableObject se puede configurar todos los parametros referentes al apartado grafico del terreno.
De esta forma, el usuario puede tener al mismo tiempo distintos TextureData para definir distintas capas y texturas.
Cabe resaltar que el terreno solo utiliza un TextureData al mismo tiempo.

El scriptableObject "TextureData" permite definir hasta 8 capas en las que se puede dividir el terreno, cada una de estas capas tiene los siguientes parametros
- **Texture**: Textura que se va a utilizar en esta sección del terreno, esta textura debe ser tileable y accesible y modificable, por lo que el sprite debe importarse con una configuración especifica, asignando el parámetro ReadOnly a false.
Cabe resaltar que no es obligatorio asignar una textura, la herramienta entonces tendra en cuenta el color elegido.
- **Tint**: La herramienta también te da la opción de tintar la capa, combinando el color seleccionado con la textura.
Tambien es una opcion no asignar un color, en este caso no se modificará el color de la textura.
- **TintStrenght**: Se constituye como un valor decimal entre 0 y 1, si el valor es cercano al 0, el color tendra menos efecto sobre la textura mientras que si es cercano al 1, la textura se vera mas afectada.
- **StartHeight**:  Este parámetro se encarga de definir la altura desde la que se empieza a renderizar esta capa. Constituye un valor decimal entre 0 y 1, siendo 0 el punto mas bajo del terreno y 1 el punto mas alto del terreno.
- **Blend Strenght**: Tambien constituye un valor entre 0 y 1. Define el suavizado de la textura de esta capa con el resto de capas. Si el valor es cercano al 0, el cambio entre las texturas de las capas es inmediato, sin nigun tipo de suavizado, mientras que si es cercano al 1, las demás capas se veran afectadas por esta de forma mas notable.
- **Texture Scale**: Define la escala con la que se renderizara la textura de esta capa.

Una vez que el usuario modifica los parametros que vea conveniente, se aplican presionando el boton de "Update" abajo del todo.
Tambien existe una alternativa que es el "AutoUpdate", que actualiza los valores internos del shader cada vez que se modifica cualquier parametro, esto se usa para hacer pequeños cambios y actualizar el terreno de forma instantanea.

![TextureData](https://github.com/Narratech/TFG-Castillo-Comas-Garcia-Villegas/assets/82290483/b5b2db79-ae8e-4ce1-ac1d-413c68538db7)




#### **Endless Terrain**

Endless terrain es un componente (Script) que permite mejorar la eficiencia de la escena con el mapa. A pesar de su nombre no genera un mapa infinito. Su funcion consiste en ocultar los chunks que se encuentran a una distancia configurable de la figura del jugador. El mapa que se puede ver ha sido generado previamente siguiendo la configuracion del Map Generator. Pero no se verá hasta ejecutar Unity.

Intrucciones:
- Si el objeto map3D tiene algo dentro debe ser eliminado
- En el objeto Map Generator hay que activar el componente Endless Terrain
- Asegurarse de que el componente anterior tiene asignado un objeto que actue de jugador
- Dar al play de Unity

#### **Map Display**

Componente que permite pintar el mapa genrado en 2D

- **TextureRenderer**: Render del objecto en el cual se va a pintar el mapa 2D

## Código de conducta 
---
El código de conducta establece las normas sociales, reglas y responsabilidades que los individuos y organizaciones deben seguir al interactuar de alguna manera con la herramienta digital o su comunidad. Es una buena práctica para crear un ambiente de respeto e inclusión en las contribuciones al proyecto. 

La plataforma Github premia y ayuda a los repositorios dispongan de este archivo. Al crear CODE_OF_CONDUCT.md puedes empezar desde una plantilla sugerida por ellos. Puedes leer más sobre cómo crear un archivo de Código de Conducta (aquí)[https://help.github.com/articles/adding-a-code-of-conduct-to-your-project/]

## Autor/es
---
Javier Comas De Frutos 

Sara Isabel Garcia Moral

Javier Enrique Villegas Montelongo

Ingacio del Castillo Rubio

## Limitación de responsabilidades
No será responsable de ningún daño directo, indirecto, incidental, especial, consecuente o punitivo, incluyendo, pero no limitado a, pérdida de ingresos, pérdida de beneficios, pérdida de datos, interrupción del negocio o cualquier otro daño similar, derivado del uso o la imposibilidad de utilizar la herramienta proporcionada. El usuario reconoce y acepta que la herramienta se proporciona "tal cual" y que Procedural OVO worlds no ofrece garantías de ningún tipo, ya sean expresas o implícitas, incluyendo, pero no limitado a, garantías de comerciabilidad, idoneidad para un propósito particular, o no infracción. Procedural OVO worlds tampoco será responsable de cualquier daño que pueda surgir como resultado de acciones realizadas por terceros. La responsabilidad total de Procedural OVO worlds, ya sea en contrato, agravio (incluyendo negligencia) o de otro modo.
