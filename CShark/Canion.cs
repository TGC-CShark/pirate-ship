using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.CShark
{
    class Canion
    {
        public TgcMesh meshCanion;
        private List<Bala> balas = new List<Bala>();
        private Bala currentBullet;

        public Canion(Vector3 pos, TgcMesh mesh)
        {
            meshCanion = mesh;
            meshCanion.Position = pos;

            for (int i = 0; i < 50; i++)
            {
                Bala bullet = new Bala(pos);
                this.balas.Add(bullet);

            }
            currentBullet = balas.First();
        }

        public void render()
        {
            meshCanion.render();

            foreach (Bala bala in balas)
            {
               bala.render();
               
            }
        }

        internal void dispose()
        {

            meshCanion.dispose();
            foreach (Bala bala in balas)
            {
                bala.dispose();
            }
        }




    }
}
