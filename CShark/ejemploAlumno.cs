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
using TgcViewer.Utils.Terrain;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils.Input;

namespace AlumnoEjemplos.CShark
{

    public enum EstadoDelJuego
    {
        Jugando,
        Instrucciones,
        Ganado,
        Perdido,
        SinEmpezar
    }

    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class EjemploAlumno : TgcExample
    {
        public static EjemploAlumno Instance { get; set; }

        public Ship ship;
        public EnemyShip shipContrincante;

        Vector3 POS_SHIP = new Vector3(0, 0, 0); //Constante
        Vector3 POS_CONTRINCANTE = new Vector3(0, 0, -1269.477f);//new Vector3(100, 0, 0); //Constante

        //Meshes
        MainCamera mainCamera;
        TgcViewer.Utils.TgcSceneLoader.TgcMesh meshShip;
        TgcViewer.Utils.TgcSceneLoader.TgcMesh meshShipContrincante;
        TgcViewer.Utils.TgcSceneLoader.TgcMesh meshCanion;
        TgcViewer.Utils.TgcSceneLoader.TgcMesh meshCanionContrincante;

        //Clima
        Lluvia lluvia = new Lluvia();
        public TgcBox lightMesh;
        public bool lloviendo;

        //Terreno
        public TerrenoSimple terrain;
        public TerrenoSimple agua;
        string currentHeightmap;
        string currentTexture;
        public float currentScaleXZ = 200f;
        public float currentScaleY = 13f;
        public TgcSkyBox skyBox;
        public TgcBox skyBoundingBox;

        public float time;
        public float heightOlas;

        public EstadoDelJuego estado;
        Menu menu;

        public Effect effect;
        public Effect efectoSombra;

        

        List<TgcMesh> meshes = new List<TgcMesh>();

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
            return "Dispararle al barco enemigo [SPACE] hasta hundirlo. Con las flechitas RIGHT y LEFT vira el barco. Con UP se acelera y DOWN desacelera. " +
                "Con A se activa la visibilidad del �ngulo de elevaci�n del disparo, y con W y S se incrementa o decrementa dicho �ngulo. " +
                "Con L se activa la tormenta.";
        }

        /// <summary>
        /// M�todo que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aqu� todo el c�digo de inicializaci�n: cargar modelos, texturas, modifiers, uservars, etc.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public override void init()
        {
            EjemploAlumno.Instance = this;

            //GuiController.Instance: acceso principal a todas las herramientas del Framework

            //Device de DirectX para crear primitivas
            Device d3dDevice = GuiController.Instance.D3dDevice;

            //Carpeta de archivos Media del alumno
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;

            
            //Terreno
            currentHeightmap = GuiController.Instance.AlumnoEjemplosMediaDir + "Heightmap3.jpg";
            currentTexture = GuiController.Instance.AlumnoEjemplosMediaDir + "TerrainTexture3.jpg";

            terrain = new TerrenoSimple();
            terrain.loadHeightmap(currentHeightmap, currentScaleXZ, currentScaleY, new Vector3(0, -125, 0));
            terrain.loadTexture(currentTexture);

            //Agua
            agua = new TerrenoSimple();
            agua.loadHeightmap(GuiController.Instance.AlumnoEjemplosMediaDir + "18_vertex_texture_02.jpg", 50f, 0.5f, new Vector3(0,-125,0));
            agua.loadTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "4141-diffuse.jpg");
            agua.AlphaBlendEnable = true;
            //heightOlas = agua.heightOlas;

            //Modifier
            GuiController.Instance.Modifiers.addFloat("heightOlas", 10, 50, 40);

            //Estado
            EjemploAlumno.Instance.estado = EstadoDelJuego.SinEmpezar;
            menu = new Menu();

            // Crear SkyBox:
            skyBox = new TgcSkyBox();
            skyBox.Center = new Vector3(0, 0, 0);
            skyBox.Size = new Vector3(12000, 12000, 12000);
            skyboxSoleado(skyBox);

            skyBoundingBox = new TgcBox();
            skyBoundingBox.Size = skyBox.Size;
            skyBoundingBox.Position = skyBox.Center;
            skyBoundingBox.AlphaBlendEnable = true;
            skyBoundingBox.updateValues();

            lloviendo = false;
            
            //Cargar meshes
            TgcViewer.Utils.TgcSceneLoader.TgcSceneLoader loader = new TgcViewer.Utils.TgcSceneLoader.TgcSceneLoader();
            TgcViewer.Utils.TgcSceneLoader.TgcScene scene = loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Vehiculos\\Canoa\\Canoa-TgcScene.xml");

            meshShip = scene.Meshes[0];
            meshShip.setColor(Color.Chocolate);

            scene = loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Vehiculos\\Canoa\\Canoa-TgcScene.xml");

            meshes.Add(meshShip);

            meshShipContrincante = scene.Meshes[0];
            meshShipContrincante.setColor(Color.BlueViolet);
            meshes.Add(meshShipContrincante);

            scene = loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Armas\\Canon\\Canon.max-TgcScene.xml");
            meshCanion =  scene.Meshes[0];
            meshes.Add(meshCanion);

            scene = loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Armas\\Canon\\Canon.max-TgcScene.xml");
            meshCanionContrincante = scene.Meshes[0];
            meshes.Add(meshCanionContrincante);

            //Shader
            effect = TgcShaders.loadEffect(alumnoMediaFolder + "shader agua.fx");
            agua.Effect = effect;
            agua.Technique = "RenderScene";
            time = 0;
            agua.AlphaBlendEnable = true;

            efectoSombra = TgcShaders.loadEffect(alumnoMediaFolder + "shader agua.fx");


            //Creaciones
            ship = new Ship(POS_SHIP, meshShip, new Canion(POS_SHIP, 5, meshCanion, true));
            shipContrincante = new EnemyShip(ship, POS_CONTRINCANTE, meshShipContrincante, new Canion(POS_CONTRINCANTE, 5, meshCanionContrincante, false));                    

            mainCamera = new MainCamera(ship);

            //Crear caja para indicar ubicacion de la luz
            lightMesh = TgcBox.fromSize(new Vector3(20, 20, 20), Color.Yellow);

        }

        public void skyboxSoleado(TgcSkyBox skyBox)
        {
            string texturesPath = GuiController.Instance.AlumnoEjemplosMediaDir + "blue sky\\";
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "bluesky_top.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "bluesky_top.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "bluesky_left.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "bluesky_right.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "bluesky_front.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "bluesky_back.jpg");
            skyBox.SkyEpsilon = 50f;
            skyBox.updateValues();
        }

        public void skyboxTormentoso(TgcSkyBox skyBox)
        {
            string texturesPath = GuiController.Instance.AlumnoEjemplosMediaDir + "grave\\";
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "grave_top.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "grave_top.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "grave_left.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "grave_right.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "grave_front.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "grave_back.jpg");
            skyBox.SkyEpsilon = 50f;
            skyBox.updateValues();
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

            TgcD3dInput input = GuiController.Instance.D3dInput;
            if (input.keyPressed(Microsoft.DirectX.DirectInput.Key.I))
            {
                estado = EstadoDelJuego.Instrucciones;
            }



            if (EjemploAlumno.Instance.estado == EstadoDelJuego.SinEmpezar)
            {
                menu.renderSinEmpezar(this);
            }

            if (EjemploAlumno.Instance.estado == EstadoDelJuego.Instrucciones)
            {
                menu.renderInstrucciones(this);
            }
                        
            if (EjemploAlumno.Instance.estado == EstadoDelJuego.Ganado)
            {
                menu.renderGanado(this);
            }

            if (EjemploAlumno.Instance.estado == EstadoDelJuego.Perdido)
            {
                menu.renderPerdido(this);
            }

            if (EjemploAlumno.Instance.estado == EstadoDelJuego.Jugando)
            {
                renderJuego(elapsedTime, d3dDevice);
            }

        }

        private void cargarLuces(Vector3 posLuz){
            Effect pointShader = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosMediaDir + "PhongShading.fx");
                        
            foreach (TgcMesh mesh in meshes){
                mesh.Effect = pointShader;
                mesh.Technique = "DefaultTechnique";//GuiController.Instance.Shaders.getTgcMeshTechnique(mesh.RenderType);

                pointShader.SetValue("fvLightPosition", TgcParserUtils.vector3ToFloat3Array(posLuz));
                pointShader.SetValue("fvEyePosition", TgcParserUtils.vector3ToFloat3Array(GuiController.Instance.ThirdPersonCamera.getPosition()));
                pointShader.SetValue("k_la", 0.55f);
                pointShader.SetValue("k_ld", 1);
                pointShader.SetValue("k_ls", 0.35f);
                pointShader.SetValue("fSpecularPower", 10);
            }
        }

        private void renderJuego(float elapsedTime, Device d3dDevice)
        {


            //Poner luz a los meshes
            Vector3 posLuz = new Vector3(POS_SHIP.X - 10000, POS_SHIP.Y + 5000, POS_SHIP.Z - 25000);
            lightMesh.Position = posLuz;

            this.cargarLuces(posLuz);

            lightMesh.render();

            //Obtener valor modifier
            heightOlas = (float)GuiController.Instance.Modifiers["heightOlas"];

            //Device device = GuiController.Instance.D3dDevice;
            time += elapsedTime;
            d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.DarkSlateBlue, 1.0f, 0);

            update(elapsedTime, time);

       

            terrain.render();

            skyBox.render();
            //skyBoundingBox.render();

            // Cargar variables de shader, por ejemplo el tiempo transcurrido.
            effect.SetValue("time", time);
            effect.SetValue("height", heightOlas);
            effect.SetValue("menorAltura", terrain.menorVerticeEnY);
            effect.SetValue("offset", (float)(terrain.Center.Y * currentScaleY));
            effect.SetValue("texHeightmap", TerrenoSimple.toTexture(currentHeightmap));
            //GuiController.Instance.Logger.log("OFFSET: " + (terrain.Center.Y * currentScaleY).ToString() + "MENOR ALTURA: " + terrain.menorVerticeEnY);

            efectoSombra.SetValue("time", time);
            efectoSombra.SetValue("height", heightOlas);
            efectoSombra.SetValue("menorAltura", terrain.menorVerticeEnY);
            efectoSombra.SetValue("offset", (float)(terrain.Center.Y * currentScaleY));
            efectoSombra.SetValue("texHeightmap", TerrenoSimple.toTexture(currentHeightmap));

            agua.heightOlas = heightOlas;
            agua.AlphaBlendEnable = true;
            agua.render();

            d3dDevice.Transform.World = Matrix.Identity;

            TgcD3dInput input = GuiController.Instance.D3dInput;
            if (input.keyPressed(Microsoft.DirectX.DirectInput.Key.L))
            {
                lloviendo = lloviendo ? false : true;
            }

            ship.renderizar();
            shipContrincante.renderizar();
            lluvia.render();
            
        }

        public void cargarEfectoLuz(Effect efecto){
            Vector3 posLuz = lightMesh.Position;

            if (lloviendo)
            {
                efecto.SetValue("fvLightPosition", TgcParserUtils.vector3ToFloat3Array(posLuz));
                efecto.SetValue("fvEyePosition", TgcParserUtils.vector3ToFloat3Array(GuiController.Instance.ThirdPersonCamera.getPosition()));
                efecto.SetValue("k_la", 0.3f);
                efecto.SetValue("k_ld", 0.3f);
                efecto.SetValue("k_ls", 0f);
                efecto.SetValue("fSpecularPower", 250);
            }
            else
            {
                efecto.SetValue("fvLightPosition", TgcParserUtils.vector3ToFloat3Array(posLuz));
                efecto.SetValue("fvEyePosition", TgcParserUtils.vector3ToFloat3Array(GuiController.Instance.ThirdPersonCamera.getPosition()));
                efecto.SetValue("k_la", 4f);
                efecto.SetValue("k_ld", 5f);
                efecto.SetValue("k_ls", 2.5f);
                efecto.SetValue("fSpecularPower", 60);
            }
        }

        public void luzAguaSoleado(TgcBox lightMesh)
        {
            luzSoleado(lightMesh);

            cargarEfectoLuz(this.effect);
            //return posLuz;
        }

        public void luzSoleado(TgcBox lightMesh)
        {
            lightMesh.Size = new Vector3(20, 20, 20);
            lightMesh.Color = Color.Yellow;
        }

        public void luzAguaTormentoso(TgcBox lightMesh)
        {             
            luzTormentoso(lightMesh);

            cargarEfectoLuz(this.effect);
            //return posLuz;
        }

        public void luzTormentoso(TgcBox lightMesh)
        {
            lightMesh.Size = new Vector3(10, 10, 10);
            lightMesh.Color = Color.Gray;
        }

        public float alturaOla(Vector3 posicion)
        {
            return agua.alturaOla(posicion, time);
        }

        private void update(float elapsedTime, float time)
        {
            ship.actualizar(elapsedTime, agua, time);
            shipContrincante.actualizar(elapsedTime, agua, time);
            mainCamera.actualizar(ship.getPosition());
        }

        /// <summary>
        /// M�todo que se llama cuando termina la ejecuci�n del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            ship.dispose();
            shipContrincante.dispose();
            terrain.dispose();
            agua.dispose();
            skyBox.dispose();
            skyBoundingBox.dispose();
            effect.Dispose();
            lightMesh.dispose();
            lluvia.dispose();
        }

    }
}
