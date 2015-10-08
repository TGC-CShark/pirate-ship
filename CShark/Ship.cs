using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.CShark
{
    public class Ship
    {
        public TgcViewer.Utils.TgcSceneLoader.TgcMesh mesh;
        static TgcD3dInput input = GuiController.Instance.D3dInput;

        const float ROTATION_SPEED = 1f;
        const float VEL_MAXIMA = 500f;
        const float ESCALON_VEL = 0.4f;
        public  float movementSpeed;
        public Vector3 vDireccion;
        public Vector3 vel;

        public float anguloRotacion = 0f;
        public float movZ;
        public float movY;
        public float movX;

        public Matrix rotacion = Matrix.Identity;
        public Matrix traslacion = Matrix.Identity;

        public Canion canion;

        private float LargoBote, AnchoBote, AltoBote;

        public float vida;

        public string nombre = "YO";
        public BarraVida barraDeVida;
        private float VIDA_MAX = 5;

        public Ship(Vector3 pos, TgcMesh mesh, Canion canion)
        {
            Vector3 size = new Vector3(15, 10, 30);
           
            this.mesh = mesh;
            this.mesh.Position = pos;

            movZ = pos.Z;
            movY = pos.Y;
            movX = pos.X;
            traslacion = Matrix.Translation(pos);

            movementSpeed = 0f;
            vel = new Vector3(0f, 0f, 0f);

            this.mesh.AutoTransformEnable = false;

            vida = VIDA_MAX;
            
            // Calcular dimensiones
            Vector3 BoundingBoxSize = mesh.BoundingBox.calculateSize();

            LargoBote = Math.Abs(BoundingBoxSize.Z);
            AnchoBote = Math.Abs(BoundingBoxSize.X);
            AltoBote = Math.Abs(BoundingBoxSize.Y);

            this.canion = canion;

            iniciarBarra();           
        }

        public Vector3 getPosition()
        {
            Vector3 pos = new Vector3(movX, movY, movZ);
            return pos;
        }

        public virtual void renderizar()
        {          
            if (tieneVida())
            {
                mesh.render();
                canion.render();
                barraDeVida.render();
            }
        }

        public virtual void actualizar(float elapsedTime, TerrenoSimple agua, float time)
        {
            calcularTraslacionYRotacion(elapsedTime);

            Matrix transformacionAgua = Matrix.Identity;// calcularPosicionConRespectoAlAgua(agua, elapsedTime, time);  
            Matrix transformacion = rotacion * traslacion;
            Matrix transformacionFinal = transformacion * transformacionAgua;

            mesh.Transform = transformacionFinal;
            
            
            actualizarCanion(anguloRotacion, elapsedTime, transformacionFinal);

            
            
        }

        public virtual void actualizarCanion(float rotacion, float elapsedTime, Matrix transf)
        {
            canion.actualizar(anguloRotacion, elapsedTime, transf);
            canion.actualizarSiEsJugador(anguloRotacion, elapsedTime, transf);
        }

        public bool tieneVida()
        {
            return vida > 0;
        }

        public virtual void calcularTraslacionYRotacion(float elapsedTime)
        {
            if (input.keyDown(Key.Left) || input.keyDown(Key.A))
            {
                anguloRotacion -= elapsedTime * ROTATION_SPEED;
                rotacion = Matrix.RotationY(anguloRotacion);
            }

            else if (input.keyDown(Key.Right) || input.keyDown(Key.D))
            {
                anguloRotacion += elapsedTime * ROTATION_SPEED;
                rotacion = Matrix.RotationY(anguloRotacion);

            }

            if (input.keyDown(Key.Up) || input.keyDown(Key.W))
            {
                movementSpeed = Math.Min(movementSpeed + ESCALON_VEL, VEL_MAXIMA);
            }

            else if (input.keyDown(Key.Down) || input.keyDown(Key.S))
            {
                movementSpeed = Math.Max(movementSpeed - ESCALON_VEL, 0);
            }

            movZ -= Convert.ToSingle(movementSpeed * Math.Cos(anguloRotacion) * elapsedTime);
            movX -= Convert.ToSingle(movementSpeed * Math.Sin(anguloRotacion) * elapsedTime);
            traslacion = Matrix.Translation(movX, 0, movZ);
        }

        public Matrix calcularPosicionConRespectoAlAgua(TerrenoSimple agua, float elapsedTime, float time)
        {
            //Estaria cheto poner cosas de la velocidad cuando sube o baja una ola

            float modificador = 1;
            if (vel.Y < -0.1f)
                modificador = 1.5f;
            if (vel.Y > 0.1f)
                modificador = (1f / 2f);

            vDireccion.Y = 0;
            vDireccion.Z = (float)Math.Cos(anguloRotacion) * elapsedTime;
            vDireccion.X = (float)Math.Sin(anguloRotacion) * elapsedTime;
            vDireccion.Normalize();

            Vector3 vDesplazamiento = vDireccion * movementSpeed * modificador;

            var nuevaPosicion = vDesplazamiento + mesh.Position;
            nuevaPosicion = new Vector3(nuevaPosicion.X, agua.aplicarOlasA(nuevaPosicion, time).Y, nuevaPosicion.Z);
            mesh.Position = nuevaPosicion;


            //Busco la nueva posicion del frente del bote
            var barcoFrente = mesh.Position;
            barcoFrente = mesh.Position + vDireccion * (LargoBote / 2);
            barcoFrente.Y = agua.aplicarOlasA(barcoFrente, time).Y;

            vel = barcoFrente - mesh.Position;
            vel.Normalize();

            return CalcularMatriz(mesh.Position, mesh.Scale, vel);
        }  

        public Vector3 vectorDireccion()
        {
            return new Vector3(- FastMath.Sin(anguloRotacion), 0, - FastMath.Cos(anguloRotacion));
        }

        public Vector3 popa()
        {
            Vector3 offsetPopa = new Vector3(FastMath.Sin(anguloRotacion), 0, FastMath.Cos(anguloRotacion));
            Vector3 popa = getPosition() + offsetPopa * (LargoBote / 2);
            popa.Y = 0;

            return popa;
        }

        // Helper tomado del ejemplo DemoShader
        private Matrix CalcularMatriz(Vector3 position, Vector3 scale, Vector3 vDireccion)
        {
            Vector3 VUP = new Vector3(0, 1, 0);

            Matrix matWorld = Matrix.Scaling(scale);
            // determino la orientacion
            Vector3 U = Vector3.Cross(VUP, vDireccion);
            U.Normalize();
            Vector3 V = Vector3.Cross(vDireccion, U);
            Matrix Orientacion;
            Orientacion.M11 = U.X;
            Orientacion.M12 = U.Y;
            Orientacion.M13 = U.Z;
            Orientacion.M14 = 0;

            Orientacion.M21 = V.X;
            Orientacion.M22 = V.Y;
            Orientacion.M23 = V.Z;
            Orientacion.M24 = 0;

            Orientacion.M31 = vDireccion.X;
            Orientacion.M32 = vDireccion.Y;
            Orientacion.M33 = vDireccion.Z;
            Orientacion.M34 = 0;

            Orientacion.M41 = 0;
            Orientacion.M42 = 0;
            Orientacion.M43 = 0;
            Orientacion.M44 = 1;
            matWorld = matWorld * Orientacion;

            // traslado
            matWorld = matWorld * Matrix.Translation(position);

            return matWorld;
        }

        public void verificarDisparo(Bala bala)
        {
            if(TgcCollisionUtils.testSphereAABB(bala.bullet.BoundingSphere, mesh.BoundingBox))
            {
                GuiController.Instance.Logger.log("LE DI!");
                reducirVida();
                bala.dispose();
            }
        }

        public virtual void iniciarBarra()
        {
            barraDeVida = new BarraVida(new Vector2(0, 0), nombre);
        }

        public virtual void reducirVida()
        {
            vida -= 1;
            barraDeVida.escalar(porcentajeDeVida());
            GuiController.Instance.Logger.log("Vida contrincante: " + vida.ToString());
        }

        public float porcentajeDeVida()
        {
            return (float)vida / (float)VIDA_MAX;
        }

        public virtual void dispose()
        {
            mesh.dispose();
            canion.dispose();
            barraDeVida.dispose();
        }
    }
}
