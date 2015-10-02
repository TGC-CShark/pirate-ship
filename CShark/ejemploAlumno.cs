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

namespace AlumnoEjemplos.CShark
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class EjemploAlumno : TgcExample
    {
        Ship ship;
        Ship shipContrincante;
        MainCamera mainCamera;
        TgcViewer.Utils.TgcSceneLoader.TgcMesh meshShip;
        TgcViewer.Utils.TgcSceneLoader.TgcMesh meshShipContrincante;

        /// <summary>
        /// Categoría a la que pertenece el ejemplo.
        /// Influye en donde se va a haber en el árbol de la derecha de la pantalla.
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
        /// Completar con la descripción del TP
        /// </summary>
        public override string getDescription()
        {
            return "barquitos lalalal";
        }

        /// <summary>
        /// Método que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aquí todo el código de inicialización: cargar modelos, texturas, modifiers, uservars, etc.
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


            //Creaciones
            Vector3 shipPos = new Vector3(0, 0, 0);
            ship = new Ship(shipPos, meshShip);
            Vector3 shipContrincantePos = new Vector3(70, 0, 0);
            shipContrincante = new Ship(shipContrincantePos, meshShipContrincante);

            mainCamera = new MainCamera(ship);

        }


        /// <summary>
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aquí todo el código referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
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
            mainCamera.actualizar(elapsedTime);
            //shipContrincante.actualizar(elapsedTime);
        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            ship.dispose();
            shipContrincante.dispose();

        }

    }
}
