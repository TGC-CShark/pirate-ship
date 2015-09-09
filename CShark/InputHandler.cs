using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.Input;

namespace AlumnoEjemplos.CShark
{
    class InputHandler
    {
        static TgcD3dInput input = GuiController.Instance.D3dInput;

        public static Vector3 getMovement(){

            Vector3 movement = new Vector3(0, 0, 0);

               if (input.keyDown(Key.Left) || input.keyDown(Key.A))
               {
                   movement.X = 1;
               }
               else if (input.keyDown(Key.Right) || input.keyDown(Key.D))
               {
                   movement.X = -1;
               }
               if (input.keyDown(Key.Up) || input.keyDown(Key.W))
               {
                   movement.Z = -1;
               }
               else if (input.keyDown(Key.Down) || input.keyDown(Key.S))
               {
                   movement.Z = 1;
               }

               return movement;
         }
    }
}
