using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YSA
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //değişkenler
        const int giris = 35, gizli = 70, cikis = 5;
        double delta = 0.8, momentum = 0.5;
        //giriş katmanı
        float[,] agirlik_giris=new float[giris,gizli];
        float[,] agirlik_gizli = new float[gizli,cikis];
        float[] esik_gizli = new float[gizli];
        float[] esik_cikis = new float[cikis];
        float[] cikis_gizli = new float[gizli];
        float[] cikis_cikis = new float[cikis];

        //istenen girisler
        //egitim sırasında doldurulacak
        float[] istenen_giris = new float[giris];
        float[] istenen_cikis = new float[cikis];

        float[] verilen_giris=new float [giris];
        float[] verilen_cikis = new float[cikis];

        float[] hata_cikis = new float[cikis];
        float[] hata_gizli = new float[gizli];

        float[,] hata_agirlik_giris = new float[giris, gizli];
        float[,] hata_agirlik_gizli = new float[gizli, cikis];
        float[] hata_esik_gizli = new float[gizli];
        float[] hata_esik_cikis = new float[cikis];

        float hata;
        float rms;
        //başlangıç değerlerinin verildiği fonksiyon
        void baslangic()
        {
            Random rnd=new Random();

            for (int i = 0; i < giris; i++)
            {
                for (int j = 0; j < gizli; j++)
                {
                    agirlik_giris[i,j]=(float)rnd.NextDouble()/10;
                }
            }

            for (int i = 0; i < gizli; i++)
            {
                for (int j = 0; j < cikis; j++)
                {
                    agirlik_gizli[i, j] = (float)rnd.NextDouble() / 10;
                }
            }

            for (int i = 0; i < gizli; i++)
            {
                esik_gizli[i] = (float)rnd.NextDouble() / 10;
            }

            for (int i = 0; i < cikis; i++)
            {
                esik_cikis[i] = (float)rnd.NextDouble() / 10;
            }
        }
        string path_agirlik = "ysa_agirlik_gizli.txt";
        string path_agirlik2 = "ysa_agirlik_cikis.txt";
        void agirlik_kayit()
        {
            File.WriteAllText(path_agirlik, "");
            FileStream fs = new FileStream(path_agirlik, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            string tmp;
            for (int i = 0; i <giris ; i++)
            {
                tmp = "";
                for (int j = 0; j < gizli; j++)
                {
                    tmp += agirlik_giris[i,j].ToString()+",";
                }
                string yazilacak=tmp.Remove(tmp.Length-1,1);
                sw.WriteLine(yazilacak);
            }
            
            sw.Flush();
            
            sw.Close();
            fs.Close();
        }
        void agirlik_kayit2()
        {
            File.WriteAllText(path_agirlik2, "");
            FileStream fs = new FileStream(path_agirlik2, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            string tmp;
            for (int i = 0; i < gizli; i++)
            {
                tmp = "";
                for (int j = 0; j < cikis; j++)
                {
                    tmp += agirlik_gizli[i, j].ToString() + ",";
                }
                string yazilacak = tmp.Remove(tmp.Length - 1, 1);
                sw.WriteLine(yazilacak);
            }
           
            sw.Flush();
            
            sw.Close();
            fs.Close();
        }
        string path = "ysa_giris.txt";
        float[] agirlik_dosyadan(int j)
        {
            float[] tmp=new float[giris];
            string[] aa=File.ReadAllLines(path);
            string[] degerler=aa[j].Split(',');
            for (int i = 0; i < giris; i++)
			{
			    tmp[i]=(float)Convert.ToDouble(degerler[i]);
			}
            return tmp;
        }
        string path2 = "ysa_cikis.txt";
        float[] cikis_dosyadan(int j)
        {
           
            float[] tmp=new float[cikis];
            string[] aa=File.ReadAllLines(path2);
            string[] degerler=aa[j].Split(',');
            for (int i = 0; i < cikis; i++)
			{
			    tmp[i]=(float)Convert.ToDouble(degerler[i]);
			}
            return tmp;
        }
        void egitim(int iter,int a)
        {
            for (int i = 0; i < iter; i++)
            {
               //istenen degerler doldurulacak
               
                for (int j = 0; j < a; j++)
			    {
                    istenen_giris = agirlik_dosyadan(j);
                    istenen_cikis = cikis_dosyadan(j);
                    ilerihesaplama();
                    float tmp = (istenen_cikis[0] - cikis_cikis[0]) + (istenen_cikis[1] - cikis_cikis[1]) + (istenen_cikis[2] - cikis_cikis[2]) + (istenen_cikis[3] - cikis_cikis[3]) + (istenen_cikis[4] - cikis_cikis[4]);
                    hata = (float)Math.Abs(tmp);
                    geriyayilim();
                    rms = rms + (float)Math.Pow(Convert.ToDouble(hata),2);
			    }
                rms = (float)Math.Sqrt(rms);
                //dosya kapatılıyor

                if (rms < 0.004)
                {
                    agirlik_kayit();
                    return;
                }
            }
        }

        //ağın çıktısı
        //öğrenmeden önce değerlerin belli olması için kullanıldı
        void ilerihesaplama()
        {
            for (int i = 0; i < gizli; i++)
            {
                float tmp = esik_gizli[i];
                for (int j = 0; j < giris; j++)
                {
                    cikis_gizli[i] = agirlik_giris[j, i] * istenen_giris[j] + tmp;
                    double abc = Convert.ToDouble(cikis_gizli[i])*-1;
                    double aa = Math.Exp(abc);
                    cikis_gizli[i] = (float)Math.Pow(aa, -1);
                }
            }

            for (int i = 0; i < cikis; i++)
            {
                float tmp = esik_cikis[i];
                for (int j = 0; j < gizli; j++)
                {
                    cikis_cikis[i] = agirlik_gizli[j, i] * cikis_gizli[j] + tmp;
                    double abc = Convert.ToDouble(cikis_cikis[i]) * -1;
                    double aa = Math.Exp(abc);
                    cikis_cikis[i] = (float)Math.Pow(aa, -1);
                }
            }
        }

        //geri yayılım algoritmasının uygulanması
        void geriyayilim()
        {
            //çıkış katmanındaki ağırlık ve eşik değerlerinin hata terimleri hesaplanır
            for (int i = 0; i < cikis; i++)
            {
                hata_cikis[i] = istenen_cikis[i] - cikis_cikis[i];
                for (int j = 0; j < gizli; j++)
                {
                    hata_agirlik_gizli[j, i] = cikis_cikis[i] * hata_cikis[i] * (1 - cikis_cikis[i]);
                    hata_esik_cikis[i] = (float)momentum * hata_agirlik_gizli[j, i] * cikis_cikis[i] + (float)delta * hata_esik_cikis[i];
                }
               
            }
            //gizli katmanındaki ağırlık ve eşik değerlerinin hata terimleri hesaplanır
            for (int i = 0; i < gizli; i++)
            {
               
                for (int j = 0; j < cikis; j++)
                {
                    hata_agirlik_giris[j, i] = cikis_gizli[i] * hata_cikis[j] * (1 - cikis_gizli[i]);
                    hata_esik_gizli[i] = (float)momentum * hata_agirlik_giris[j, i] * cikis_gizli[i] + (float)delta * hata_esik_gizli[i];
                }

            }

            //eşik değerleri güncelleniyor
            for (int i = 0; i < cikis; i++)
            {
                esik_cikis[i] = esik_cikis[i] + hata_esik_cikis[i];
                for (int j = 0; j < gizli; j++)
                {
                    agirlik_gizli[j, i] = agirlik_gizli[j, i] + hata_agirlik_gizli[j,i];
                }

            }

            //ağırlık değerleri güncelleniyor
            for (int i = 0; i < gizli; i++)
            {
                esik_gizli[i] = esik_gizli[i] + hata_esik_gizli[i];
                for (int j = 0; j < giris; j++)
                {
                    agirlik_giris[j, i] = agirlik_giris[j, i] + hata_agirlik_giris[j, i];
                }

            }
        }

        private void button_egitim_Click(object sender, EventArgs e)
        {
            egitim(10,5);
            MessageBox.Show("Eğitim tamamlandı");
        }

        private void button_hesapla_Click(object sender, EventArgs e)
        {
            baslangic();
            ilerihesaplama();
            lbl_a.Text = cikis_cikis[0].ToString();
            lbl_b.Text = cikis_cikis[1].ToString();
            lbl_c.Text = cikis_cikis[2].ToString();
            lbl_d.Text = cikis_cikis[3].ToString();
            lbl_e.Text = cikis_cikis[4].ToString();
            
        }

        private void button_giris(object sender, EventArgs e)
        {
           Button btn=(Button)sender;
           string ad = btn.Name;
           ad = ad.Split('_')[1];
           int i=Convert.ToInt32(ad);
           i--;
           if (istenen_giris[i] == 0)
           {
               istenen_giris[i] = 1;
               btn.BackColor = Color.Green;
           }
           else
           {
               istenen_giris[i] = 0;
               btn.BackColor = btn.BackColor = Color.White;
           }
        }
 
    }
}
