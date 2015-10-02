using Microsoft.DirectX;
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
            TgcBox box;
            public TgcViewer.Utils.TgcSceneLoader.TgcMesh mesh;
            public Vector3 position { get; set; }
            Vector3 size;
            float movementSpeed=50;
            Vector3 movement;
            static TgcD3dInput input = GuiController.Instance.D3dInput;
            const float ROTATION_SPEED = 1f;


        public Ship(Vector3 pos, TgcMesh mesh)
            {
                this.position = pos;
                Vector3 size = new Vector3(15, 10, 30);
                this.mesh = mesh;
                /*Color color = Color.Beige;
                box = TgcBox.fromSize(position, size, color);*/
               
        }


        public void renderizar(){
            //box.render();
            mesh.render();
            }

           public void actualizar(float elapsedTime){

              
            if (input.keyDown(Key.Left) || input.keyDown(Key.A))
                {
                mesh.rotateY(ROTATION_SPEED * elapsedTime);
                //box.rotateY(ROTATION_SPEED * elapsedTime);
            }
                else if (input.keyDown(Key.Right) || input.keyDown(Key.D))
                {
                mesh.rotateY(-ROTATION_SPEED * elapsedTime);
                //box.rotateY(-ROTATION_SPEED * elapsedTime);
            }
            position = mesh.BoundingBox.Position;
            //position = box.BoundingBox.Position;

            movement = InputHandler.getMovement();
            movement *= movementSpeed * elapsedTime;
            mesh.move(movement);
            //box.move(movement);

        }



           internal void dispose()
           {
            //box.dispose();
            mesh.dispose();
           }
    }
}
