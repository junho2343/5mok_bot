using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace 진짜오목
{  
    public partial class Form1 : Form
    {

      

        Unit u;

        Graphics g;

        SolidBrush brush;

        Random r = new Random();


        // 돌들의 위치가 저장될 값
        int[,] Stons = new int[19, 19];

        int[,] Black = new int[19, 19];


        public Form1()
        {
            InitializeComponent();

            u = new Unit() { One_size = 30, Stone_size = 28 };


            ClientSize = new Size(u.One_size * 18, u.One_size * 18);

            for (int i = 0; i < 19; i++)
            {
                for (int j = 0; j < 19; j++)
                {
                    Stons[i, j] = 0;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint(e);
            g = CreateGraphics();

            Pen pen = new Pen(Color.Black);

            for (int i = 0; i < 19; i++)
            {

                //가로
                g.DrawLine(pen, 0, i * u.One_size, u.One_size * 18, i * u.One_size);

                //세로
                g.DrawLine(pen, i * u.One_size, 0 , i * u.One_size, u.One_size * 18);

            }
        }

        async private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            //사용자가 누를때
            int y = (e.X + u.One_size / 2) / u.One_size;
            int x = (e.Y+ u.One_size / 2) / u.One_size;

            // 이미 돌을 놨을때
            if (Stons[x, y] > 0) return;
            
            await DrawStone(x, y, "Black");


            // 봇의차례
            Bot_Click();

            


        }
        private async Task<int> DrawStone(int x, int y, string type)
        {


            if(type == "Black")
            {
                Black[x, y] = 100;
                brush = new SolidBrush(Color.Black);
            }else
            {
                Black[x, y] = 50;
                brush = new SolidBrush(Color.White);
            }


            g.FillEllipse(brush, new Rectangle(y * u.One_size - u.One_size / 2, x * u.One_size - u.One_size / 2, u.Stone_size, u.Stone_size));

            Stons[x, y] = 1000000000;

            await Task.Delay(100);


            // 클릭한 주위의 값들을 낮춤
            if (x == 0 || y == 0 || x == 18 || y == 18)
            {

            }
            else
            {
                Stons[x, y - 1] -= 1;
                Stons[x + 1, y - 1] -= 1;
                Stons[x - 1, y - 1] -= 1;
                Stons[x - 1, y + 1] -= 1;
                Stons[x - 1, y] -= 1;

                Stons[x + 1, y] -= 1;
                Stons[x + 1, y + 1] -= 1;
                Stons[x, y + 1] -= 1;
            }

            // 오목완성 체크



                int cnt1; // 가로체크 cnt
                int cnt2; // 세로체크 cnt
                int cnt3; // 대각선 ↘ cnt
                int cnt4; // 대각선 ↙ cnt
            
                int before1; // 가로체크 before
                int before2; // 세로체크 before
                int before3; // 대각선 ↘ before
                int before4; // 대각선 ↙ before

                string text;


                for (int i = 0; i < 19; i++)
                {
                    cnt1 = 1;
                    cnt2 = 1;
                    
                    before1 = 10;
                    before2 = 10;

                    for (int j = 0; j < 19; j++)
                    {
                        cnt3 = 0;
                        cnt4 = 0;   

                        // 가로 우선순위 찾기
                        if (Stons[i, j] > 0 && Black[i, j] == before1)
                            cnt1++;
                        else
                            cnt1 = 1;

                        before1 = Black[i, j];
                        ///////////////////////
                        
                        // 세로순위 우선순위 찾기
                        if (Stons[j, i] > 0 && Black[j, i] == before2)
                            cnt2++;
                        else
                            cnt2 = 1;

                        before2 = Black[j,i];
                        ///////////////////////
                        

                        // 대각선 ↘ 순위 우선순위 찾기
                        if (i + 4 <= 18 && j + 4 <= 18) {

                            before3 = Black[i, j];
                            
                            for (int k = 0; k < 5; k++)
			                {
                                if(Stons[i + k, j + k] > 0 && Black[i + k, j + k] == before3)
                                    cnt3++;
                           
			                }
                        }
                        ///////////////////////

                        // 대각선 ↙ 순위 우선순위 찾기
                        if (i + 4 <= 18 && j - 4 >= 0) {

                            before4 = Black[i, j];
                            
                            for (int k = 0; k < 5; k++)
			                {
                                if(Stons[i + k, j - k] > 0 && Black[i + k, j - k] == before4)
                                    cnt4++;
			                }
                        }
                        ///////////////////////
                        

                        //오목체크
                        if (cnt1 >= 5 || cnt2 >= 5 || cnt3 >= 5 || cnt4 >= 5)
                        {
                            //Debug.WriteLine("{0} {1}", i, j);
                            text = Black[i, j] == 100 ? "검은돌" : "흰돌";
                            text += " 승리!!!";

                            MessageBox.Show(text);
                            Application.Exit();
                        }


                        
                        // 3개를 우선순위 -10, 4개이상은 우선순위 -20

                        // 가로, 세로  j - 3 >= 0 && j + 1 <= 18 돌이 배열 범위를 벗어나는지 체크
                        if (j - 3 >= 0 && j + 1 <= 18) {

                            //가로값
                            if (cnt1 == 3)
                            {
                                Stons[i, j - 3] -= 10;
                                Stons[i, j + 1] -= 10;
                            }else if (cnt1 > 3)
                            {
                                Stons[i, j - 3] -= 20;
                                Stons[i, j + 1] -= 20;
                            }

                            //세로값
                            if (cnt2 == 3)
                            {
                                Stons[j - 3, i] -= 10;
                                Stons[j + 1, i] -= 10;
                            }else if (cnt2 > 3)
                            {
                                Stons[j - 3, i] -= 20;
                                Stons[j + 1, i] -= 20;
                            }
                        }     
                      
                        // 대각선 ↘ 값
                        if ( i + 4 <= 18 && j + 4 <= 18) {
                            
                            if (cnt3 == 3){
                                if (i - 1 >= 0 && j - 1 >= 0 )
                                    Stons[i - 1, j - 1] -= 10;
                                Stons[i + 3, j + 3] -= 10;
                            }else if (cnt3 > 3) {
                                if (i - 1 >= 0 && j - 1 >= 0 )
                                    Stons[i - 1, j - 1] -= 20;
                                Stons[i + 4, j + 4] -= 20;
                            }
                           
                        }
                        // 대각선 ↙ 값
                        if ( i + 4 <= 18 && j - 4 >= 0) {
                            
                            if (cnt4 == 3){
                                if (i - 1 >= 0 && j + 1 <= 18)
                                    Stons[i - 1, j + 1] -= 10;
                                Stons[i + 3, j - 3] -= 10;
                            }else if (cnt4 > 3) {
                                if (i - 1 >= 0 && j + 1 <= 18)
                                    Stons[i - 1, j + 1] -= 20;
                                Stons[i + 4, j - 4] -= 20;
                            }

                        }
                       
                    }

                }

       
            return 1;
            //세로체크
            // for (int i = 0; i < 18; i++)
            //         {
            //             bol = false;
            //             cnt = 0;

            //             for (int j = 0; j < 18; j++)
            //             {
            //                 if (Stons[j, i] > 0)
            //                     cnt++;
            //                 else
            //                     cnt = 0;

            //                 if (cnt == 3)
            //                 {
            //                     Stons[j, i - 3] = -50;
            //                     Stons[j, i + 1] = -50;
            //                 }
            //             }

            //         }
        }
        async private void Bot_Click()
        {
            // 가장 낮은 수들을 먼저 놓는다.

            List<Bot> location = new List<Bot>();
            int temp = 100;
            
            for (int i = 0; i < 19; i++)
            {
                for (int j = 0; j < 19; j++)
                {
                    if(Stons[i, j] < temp)
                    {
                        //가장 작은값이라면 초기화 후 추가
                        location.Clear();
                        location.Add(new Bot { X = i, Y = j });
                        temp = Stons[i, j];

                    }else if (Stons[i, j] == temp) 
                    {
                         //가장 작은값과 같다면 추가
                        location.Add(new Bot { X = i, Y = j });
                    }
                }
            }

            foreach (var item in location)
            {
            }

            int length = location.Count();

            if ( length == 1)
            {
                // 경우의 수가 한가지일 경우 
                await DrawStone(location[0].X, location[0].Y, "White");
            }
            else
            {
                // 경우의 수가 여러가지일 경우 랜덤
                int rnd = r.Next(0, length - 1);
                await DrawStone(location[rnd].X, location[rnd].Y, "White");
                
            }

        }
    }
    class Unit
    {
        public int Stone_size {get; set;}
        public int One_size {get; set;}
    }
    class Bot
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
    
   
}
