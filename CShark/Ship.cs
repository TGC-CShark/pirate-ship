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
        public Vector3 position { get; set; }
        Vector3 size;
        Vector3 movement;
        static TgcD3dInput input = GuiController.Instance.D3dInput;

        const float ROTATION_SPEED = 1f;
        const float movementSpeed = 0.25f;

        float anguloRotacion = 0f;
        float movX;// = 0f;
        float movY;// = 0f;

        Matrix rotacion = Matrix.Identity;
        Matrix traslacion;// = Matrix.Identity;

        private Canion canion;

        public string nombre = "ship";

      
        public Ship(Vector3 pos, TgcMesh mesh, Canion canion)
        {
            this.position = pos;
            Vector3 size = new Vector3(15, 10, 30);
            this.mesh = mesh;
            this.mesh.Position = pos;

            movX = pos.X;
            movY = pos.Y;
            traslacion = Matrix.Translation(pos);

            this.mesh.AutoTransformEnable = false;

            this.canion = canion;
            
        }


        public void renderizar()
        {
            mesh.render();
            canion.render();
                       
        }

        public void actualizar(float elapsedTime)
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
                movX -= Convert.ToSingle(movementSpeed * Math.Cos(anguloRotacion));
                movY -= Convert.ToSingle(movementSpeed * Math.Sin(anguloRotacion));
                traslacion = Matrix.Translation(movY, 0, movX);
            }

            else if (input.keyDown(Key.Down) || input.keyDown(Key.S))
            {
                movX += Convert.ToSingle(movementSpeed * Math.Cos(anguloRotacion));
                movY += Convert.ToSingle(movementSpeed * Math.Sin(anguloRotacion));
                traslacion = Matrix.Translation(movY, 0, movX);
            }

            Matrix transformacion = rotacion * traslacion;

            mesh.Transform = transformacion;
            canion.meshCanion.Transform = transformacion;

        }

        
        internal void dispose()
        {

            mesh.dispose();
            canion.dispose();
        }
    }
}
