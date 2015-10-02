using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.CShark
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class EjemploAlumno : TgcExample
    {
        Ship ship;
        EnemyShip shipContrincante;

        Vector3 POS_SHIP = new Vector3(0, 0, 0); //Constante
        Vector3 POS_CONTRINCANTE = new Vector3(100, 0, 0); //Constante

        MainCamera mainCamera;
        TgcViewer.Utils.TgcSceneLoader.TgcMesh meshShip;
        TgcViewer.Utils.TgcSceneLoader.TgcMesh meshShipContrincante;
        TgcViewer.Utils.TgcSceneLoader.TgcMesh meshCanion;
        TgcViewer.Utils.TgcSceneLoader.TgcMesh meshCanionContrincante;

        /// <summary>
        /// Categor�a a la que pertenece el ejemplo.
        /// Influye en donde se va a haber en el �rbol de la derecha de la pantalla.
        /// </summary>
        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        /// <summary>
        /// Completar nombre del grupo en formato Grupo NN
        /// </summary>
        public override string getName()
        {
            return "CShark";
        }

        /// <summary>
        /// Completar con la descripci�n del TP
        /// </summary>
        public override string getDescription()
        {
            return "barquitos lalalal";
        }

        /// <summary>
        /// M�todo que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aqu� todo el c�digo de inicializaci�n: cargar modelos, texturas, modifiers, uservars, etc.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public override void init()
        {
            //GuiController.Instance: acceso principal a todas las herramientas del Framework

            //Device de DirectX para crear primitivas
            Device d3dDevice = GuiController.Instance.D3dDevice;

            //Carpeta de archivos Media del alumno
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;

            //Cargar meshes
            TgcViewer.Utils.TgcSceneLoader.TgcSceneLoader loader = new TgcViewer.Utils.TgcSceneLoader.TgcSceneLoader();
            TgcViewer.Utils.TgcSceneLoader.TgcScene scene = loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Vehiculos\\Canoa\\Canoa-TgcScene.xml");

            meshShip = scene.Meshes[0];
            meshShip.setColor(Color.Chocolate);

            scene = loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Vehiculos\\Canoa\\Canoa-TgcScene.xml");

            meshShipContrincante = scene.Meshes[0];
            meshShipContrincante.setColor(Color.BlueViolet);

            scene = loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Armas\\Canon\\Canon.max-TgcScene.xml");

            meshCanion =  scene.Meshes[0];
            meshCanionContrincante = scene.Meshes[0];


            //Creaciones
            shipContrincante = new EnemyShip(POS_CONTRINCANTE, meshShipContrincante, new Canion(new Vector3(70, 0, 50), meshCanion));
            ship = new Ship(POS_SHIP, meshShip, new Canion(new Vector3(0, 0, 0), meshCanionContrincante));
                    

            mainCamera = new MainCamera(ship);

        }


        /// <summary>
        /// M�todo que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aqu� todo el c�digo referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el �ltimo frame</param>
        public override void render(float elapsedTime)
        {
            //Device de DirectX para renderizar
            Device d3dDevice = GuiController.Instance.D3dDevice;

            update(elapsedTime);
             ship.renderizar();
            shipContrincante.renderizar();

      
            d3dDevice.Transform.World = Matrix.Identity;
        }

        private void update(float elapsedTime)
        {
            ship.actualizar(elapsedTime);
            shipContrincante.actualizar(elapsedTime);
            mainCamera.actualizar(ship.position);
        }

        /// <summary>
        /// M�todo que se llama cuando termina la ejecuci�n del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            ship.dispose();
            shipContrincante.dispose();

        }

    }
}
