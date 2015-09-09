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

namespace AlumnoEjemplos.CShark
{
    class Ship
    {
            TgcBox box;
            public Vector3 position { get; set; }
            Vector3 size;
            float movementSpeed=50;
            Vector3 movement;
         

            public Ship(Vector3 pos)
            {
                this.position = pos;
                Vector3 size = new Vector3(15, 10, 30);
                Color color = Color.Brown;
                box = TgcBox.fromSize(position, size, color);
                
            }

            public void renderizar(){
                box.render();
            }

           public void actualizar(float elapsedTime){

               movement = InputHandler.getMovement();
               movement *= movementSpeed * elapsedTime;
               box.move(movement);
               position = box.BoundingBox.Position;
           }



           internal void dispose()
           {
               box.dispose();
           }
    }
}
