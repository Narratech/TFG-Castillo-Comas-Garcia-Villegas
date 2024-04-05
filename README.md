# TFG-Castillo-Comas-Garcia-Villegas
<h1 align="center"> Procedural OVO Worlds </h1>
<p align="center"> Logo e imagen o gif de la interfaz principal de la herramienta</p>
<p align="center"><img src="https://www.webdevelopersnotes.com/wp-content/uploads/create-a-simple-home-page.png"/></p> 

## Tabla de contenidos:
---

- [Badges](#badges)
- [Descripción y contexto](#descripción-y-contexto)
- [Guía de instalación](#guía-de-instalación)
- [Guía de usuario](#guía-de-usuario)
- [Código de conducta](#código-de-conducta)
- [Autor/es](#autores)
- [Información adicional](#información-adicional)
- [Licencia](#licencia)
- [Limitación de responsabilidades - Solo BID](#limitación-de-responsabilidades)

## Badges
--- 
- Movimiento y camara de ejemplo para moverse por el mapa: [Modular First Person Controller](https://assetstore.unity.com/packages/3d/characters/modular-first-person-controller-189884)
- Unity Version 2022.3.1f1 LTS y **Posteriores?¿** [![Made with Unity](https://img.shields.io/badge/Made%20with-Unity-57b9d3.svg?style=flat&logo=unity)](https://unity3d.com)
  
### Badges que solicitamos:
---
Para el mejor y correcto uso de la herramienta solicitamos a los equipos que sumen sus herramientas al catálogo  de sumar el badge por el uso del microservicio de Procedural OVO Worlds.

El badge se ve así y redirige al reporte de evaluación estática del último commit de la herramienta:

**PONER LINKASO A NUESTRO REPO DE DESCARGA, ASSET STORE LO K SEA**

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=EL-BID_Plantilla-de-repositorio&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=EL-BID_Plantilla-de-repositorio)


## Descripción y contexto
---
Procedural OVO Worlds es una herramienta de creacion de mapas procedurales 3D, es una potente solucion que permite generar sus propios mapas personalizados con una gran variedad de estilos y elementos.
Principalmente la herramienta proporciona dos estilos principales: **Cartoon y Cúbico**(tipo Minecraft), proporcionando asi opciones visuales diversificadas para satisfacer las preferencias de los usuarios.

En cuanto a la generacion y estilos de terrenos,esta es capaz de generar una amplia gama de estilos, permitiendo asi adaptarse a multiples/diversas tematicas y necesidades creativas del usuario. Los tipos de terreno que genera son biomas y estos determinan las caracteristicas de este tipo de terreno.
Por otro lado la herramienta dispone de dos sistemas de generacion de objectos, el sistema de generacion de puntos de interes el cual sirve para genrar objectos que se les quiere dar una mayor importancia y por otro lado el 
sistema de folliage para poblar el mapa generado con estos objectos.

## Guía de instalación
---
Paso a paso de cómo instalar la herramienta digital. En esta sección es recomendable explicar la arquitectura de carpetas y módulos que componen el sistema.

Según el tipo de herramienta digital, el nivel de complejidad puede variar. En algunas ocasiones puede ser necesario instalar componentes que tienen dependencia con la herramienta digital. Si este es el caso, añade también la siguiente sección.

La guía de instalación debe contener de manera específica:
- Los requisitos del sistema operativo para la compilación (versiones específicas de librerías, software de gestión de paquetes y dependencias, SDKs y compiladores, etc.).
- Las dependencias propias del proyecto, tanto externas como internas (orden de compilación de sub-módulos, configuración de ubicación de librerías dinámicas, etc.).
- Pasos específicos para la compilación del código fuente y ejecución de tests unitarios en caso de que el proyecto disponga de ellos.

## Guía de usuario
---

En primer luegar encontramos el prefab **Map Generator**, ene ste podemos encontrar el **componente principal** de la herrmienta que es el **Map Generator**.

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
- **Auto Regenerate**: *DEBERIAMOS QUITARLO ?¿*
- **Generate Objects**: Booleando que permite la creacion de gameobjects en el mapa
- **Generate InterestPoints**: Booleando que permite la creacion de los puntos de interes anteriormente establecidos en el mapa.

### Dependencias
Descripción de los recursos externos que generan una dependencia para la reutilización de la herramienta digital (librerías, frameworks, acceso a bases de datos y licencias de cada recurso). Es una buena práctica describir las últimas versiones en las que ha sido probada la herramienta digital. 

    Puedes usar este estilo de letra diferenciar los comandos de instalación.

## Código de conducta 
---
El código de conducta establece las normas sociales, reglas y responsabilidades que los individuos y organizaciones deben seguir al interactuar de alguna manera con la herramienta digital o su comunidad. Es una buena práctica para crear un ambiente de respeto e inclusión en las contribuciones al proyecto. 

La plataforma Github premia y ayuda a los repositorios dispongan de este archivo. Al crear CODE_OF_CONDUCT.md puedes empezar desde una plantilla sugerida por ellos. Puedes leer más sobre cómo crear un archivo de Código de Conducta (aquí)[https://help.github.com/articles/adding-a-code-of-conduct-to-your-project/]

## Autor/es
---
Nombra a el/los autor/es original/es. Consulta con ellos antes de publicar un email o un nombre personal. Una manera muy común es dirigirlos a sus cuentas de redes sociales.

## Información adicional
---
Esta es la sección que permite agregar más información de contexto al proyecto como alguna web de relevancia, proyectos similares o que hayan usado la misma tecnología.

## Licencia 
---

La licencia especifica los permisos y las condiciones de uso que el desarrollador otorga a otros desarrolladores que usen y/o modifiquen la herramienta digital.

Incluye en esta sección una nota con el tipo de licencia otorgado a esta herramienta digital. El texto de la licencia debe estar incluído en un archivo *LICENSE.md* o *LICENSE.txt* en la raíz del repositorio.

Si desconoces qué tipos de licencias existen y cuál es la mejor para cada caso, te recomendamos visitar la página https://choosealicense.com/.

Si la herramienta que estás publicando con la iniciativa Código para el Desarrollo ha sido financiada por el BID, te invitamos a revisar la [licencia oficial del banco para publicar software](https://github.com/EL-BID/Plantilla-de-repositorio/blob/master/LICENSE.md)

## Limitación de responsabilidades
Disclaimer: Esta sección es solo para herramientas financiadas por el BID.

El BID no será responsable, bajo circunstancia alguna, de daño ni indemnización, moral o patrimonial; directo o indirecto; accesorio o especial; o por vía de consecuencia, previsto o imprevisto, que pudiese surgir:

i. Bajo cualquier teoría de responsabilidad, ya sea por contrato, infracción de derechos de propiedad intelectual, negligencia o bajo cualquier otra teoría; y/o

ii. A raíz del uso de la Herramienta Digital, incluyendo, pero sin limitación de potenciales defectos en la Herramienta Digital, o la pérdida o inexactitud de los datos de cualquier tipo. Lo anterior incluye los gastos o daños asociados a fallas de comunicación y/o fallas de funcionamiento de computadoras, vinculados con la utilización de la Herramienta Digital.
