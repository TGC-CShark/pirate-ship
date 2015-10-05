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
    class Ship
    {
        public TgcViewer.Utils.TgcSceneLoader.TgcMesh mesh;
        //public Vector3 position { get; set; }
        Vector3 size;
        Vector3 movement;
        static TgcD3dInput input = GuiController.Instance.D3dInput;

        const float ROTATION_SPEED = 1f;
        const float VEL_MAXIMA = 500f;
        const float ESCALON_VEL = 0.4f;
        float movementSpeed;
        public Vector3 vDireccion;

        float anguloRotacion = 0f;
        public float movZ;
        public float movY;
        public float movX;

        public Matrix rotacion = Matrix.Identity;
        public Matrix traslacion;

        public Canion canion;

        public string nombre = "ship";

      
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

            this.mesh.AutoTransformEnable = false;

            this.canion = canion;
            
        }

        public Vector3 getPosition()
        {
            Vector3 pos = new Vector3(movX, movY, movZ);
            return pos;
        }


        public void renderizar()
        {
            mesh.render();
            canion.render();
                       
        }

        public void actualizar(float elapsedTime, TerrenoSimple agua, float time)
        {
            calcularTraslacionYRotacion(elapsedTime);

            Matrix transformacion = rotacion * traslacion;

            mesh.Transform = transformacion;
            canion.meshCanion.Transform = transformacion;

            Matrix transformacionAgua = calcularPosicionConRespectoAlAgua(agua, elapsedTime, time);

            mesh.Transform = transformacionAgua;
            canion.meshCanion.Transform = transformacionAgua;

            canion.actualizar(anguloRotacion, elapsedTime, getPosition());

        }

        public void calcularTraslacionYRotacion(float elapsedTime)
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

        private Matrix calcularPosicionConRespectoAlAgua(TerrenoSimple agua, float elapsedTime, float time)
        {
            //Estaria cheto poner cosas de la velocidad cuando sube o baja una ola

            vDireccion.Y = 0;
            vDireccion.Z = (float)Math.Cos(anguloRotacion) * elapsedTime;
            vDireccion.X = (float)Math.Sin(anguloRotacion) * elapsedTime;
            vDireccion.Normalize();

            Vector3 vDesplazamiento = vDireccion * movementSpeed;

            var nuevaPosicion = vDesplazamiento + mesh.Position;
            nuevaPosicion = new Vector3(nuevaPosicion.X, agua.aplicarOlasA(nuevaPosicion, time).Y, nuevaPosicion.Z);
            mesh.Position = nuevaPosicion;

            return CalcularMatriz(mesh.Position, mesh.Scale, vDireccion);

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

        internal void dispose()
        {

            mesh.dispose();
            canion.dispose();
        }
    }
}
