# WsNetCore
Implementation of websockets - NetCore + Angular

-Se Desarrollo una applicacion para tratar la funcionalidad de websockets integradas con NetCore y Angular.
-Al iniciar la aplicacion es encuentran una serie de textboxs donde se pueden cambiar el tamañde un notepad, asi como abrirlo y cerrarlo. es bastante sencilla de entender.

-Al ejecutar el server se abrirá una ventana (localhost:5001) en el browser para poder ser utilizada, al iniciar el app desde angular con ng serve (localhost:4200) se abrira otra ventana
de browser igual a la anterior mencionada.

-Al utilizar cualquiera de las 2 en la otra se vera reflejado un texto de los usuarios que esten activos.

-Para mover el notepad es necesario mover el cuadro que se encuentra dentro del browser.

**#Front en Angular**

-Abrir la carpeta Client-App, se requiere instalar Node modules con **npm install**
Ejecutar Ng serve y navegar a http://localhost:4200/

**#BackEnd VisualStudio**

Si el proyecto no corre, verificar que se encuentre instalado Node Modules en el front y que se este ejecutando en **WebSocketAndNetCore.WEB** al momento de presionar el boton "Play" (que por defecto casi siempre este en "IIS").
