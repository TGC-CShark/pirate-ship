using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.CShark
{
    class Bala
    {
        private TgcSphere bullet;
        private bool visible = false;

        public Bala(Vector3 pos)
        {

            bullet = new TgcSphere();
            bullet.setColor(Color.Black);
            bullet.Radius = 4f;
            bullet.Position = pos;
            bullet.LevelOfDetail = 1;
            bullet.updateValues();

        }

        public void render()
        {
            if (visible) { bullet.render(); }
            
        }

        public void dispose()
        {
            bullet.dispose();
        }


    }
}
