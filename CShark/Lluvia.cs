﻿using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.CShark
{
    class Lluvia : TgcAnimatedSprite
    {

        public Lluvia() : base(GuiController.Instance.AlumnoEjemplosMediaDir + "LLUVIA1.png", new Size(128, 128),32768,20)
        {
            Position = new Vector2(-10, 0);
            Scaling = new Vector2(12, 8);

        }

        new public void render()
        {
            if (EjemploAlumno.Instance.lloviendo)
            {

                 
                GuiController.Instance.Drawer2D.beginDrawSprite();
                updateAndRender();
                GuiController.Instance.Drawer2D.endDrawSprite();
            }
        }
            
    }
}
