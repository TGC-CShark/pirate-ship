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
            Vector3 position;
            Vector3 size;
            float movementSpeed=50;
            Vector3 movement;

            public Ship(Vector3 pos)
            {
                position = pos;
                Vector3 size = new Vector3(10, 10, 10);
                Color color = Color.Red;
                box = TgcBox.fromSize(position, size, color);
                
            }

            public void renderizar(){
                box.render();
            }

           public void actualizar(float elapsedTime){

               movement = InputHandler.getMovement();
               movement *= movementSpeed * elapsedTime;
               box.move(movement);
           }



           internal void dispose()
           {
               box.dispose();
           }
    }
}
