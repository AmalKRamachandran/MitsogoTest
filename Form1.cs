﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Net.NetworkInformation;
using System.Collections.ObjectModel;
using ManagedNativeWifi;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using System.Device.Location;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Data.Common;
using Quartz.Impl;
using Quartz;

namespace test
{
    public partial class Form1 : Form
    {

        private GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();
        string latitude = "0";
        string longitute = "0";
        string OnConnection = @"Data Source=192.168.15.97\SQLEXPRESS;Initial Catalog=test;User ID=sa;Password=admin@123";
        string weather = "";
        int min;
        public Form1()
        {
            InitializeComponent();
            loadtimeincombo();
            GetSettings();


        }

        public void loadtimeincombo()
        {
            try
            {
                DataTable Inervel = new DataTable();
                Inervel.Columns.Add("value", typeof(string));
                Inervel.Columns.Add("Name", typeof(string));
                Inervel.Rows.Add("5", "5 Minutes");
                Inervel.Rows.Add("10", "10 Minutes");
                Inervel.Rows.Add("15", "15 Minutes");
                Inervel.Rows.Add("20", "20 Minutes");
                Inervel.Rows.Add("30", "30 Minutes");
                Inervel.Rows.Add("45", "45 Minutes");
                Inervel.Rows.Add("60", "1 Hour");
                Inervel.Rows.Add("120", "2 Hour");
                Inervel.Rows.Add("180", "3 Hour");
                Inervel.Rows.Add("240", "4 Hour");
                Inervel.Rows.Add("300", "5 Hour");
                comboBoxinterval.ValueMember = "value";
                comboBoxinterval.DisplayMember = "Name";
                comboBoxinterval.DataSource = Inervel;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public void get_details()
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }



        public void LoadGrid()
        {
            try
            {
                SqlConnection con = new SqlConnection(OnConnection);
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();
                DateTime Forday = Convert.ToDateTime(txtDateFrom.Value);
                string ForDate = string.Format("{0:M/d/yyyy}", Forday);

                SqlDataReader dataReader;
                SqlCommand cmd = new SqlCommand("select [Date],[battery],[device],[wifi],[storage],[weather] from details where  convert(date, Date) = '" + ForDate + "'", con);
                dataReader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dataReader);
                dataGridView1.DataSource = dt;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public void GetSettings()
        {
            try
            {
                SqlConnection con = new SqlConnection(OnConnection);
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();
                SqlDataReader dataReader;
                SqlCommand cmd = new SqlCommand("select * from properties", con);
                dataReader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dataReader);

                if( dt.Rows[1]["status"].ToString()=="1")
                {
                    chkBattery.Checked = true;
                }
                else
                {
                    chkBattery.Checked = false;
                }


                if (dt.Rows[2]["status"].ToString() == "1")
                {
                    chkDevice.Checked = true;
                }
                else
                {
                    chkDevice.Checked = false;
                }


                if (dt.Rows[3]["status"].ToString() == "1")
                {
                    chkWifi.Checked = true;
                }
                else
                {
                    chkWifi.Checked = false;
                }


                if (dt.Rows[4]["status"].ToString() == "1")
                {
                    chkStorage.Checked = true;
                }
                else
                {
                    chkStorage.Checked = false;
                }


                if (dt.Rows[5]["status"].ToString() == "1")
                {
                    chkWeather.Checked = true;
                }
                else
                {
                    chkWeather.Checked = false;
                }

               comboBoxinterval.SelectedValue = dt.Rows[0]["status"].ToString();

                min = Convert.ToInt32( comboBoxinterval.SelectedValue);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }




        public string  Battery()
        {
            string Battery = "";
            try
            {
                if (chkBattery.Checked)
                {

                    ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_Battery");

                    foreach (ManagementObject mo in mos.Get())
                    {

                        Battery = mo["EstimatedChargeRemaining"].ToString() + "%";
                    }

                }
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
             return Battery;
            
        }

        public string os ( )
        {
            string os="";
            try
            {
                if (chkBattery.Checked)
                {
                    var name = (from x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get().Cast<ManagementObject>()
                                select x.GetPropertyValue("Caption")).FirstOrDefault();
                    string xq = name != null ? name.ToString() : "Unknown";
                    os = Environment.MachineName.ToString() + " OS:" + xq;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return os;
        }

        public string GetDiskspace()
        {
            string Disk = "";
            try
            {
                if (chkStorage.Checked)
                {
                    string details = "";
                    foreach (DriveInfo drive in DriveInfo.GetDrives())
                    {
                        if (drive.IsReady)
                        {
                            details = details + Environment.NewLine + "Drive Name: " + drive.Name.ToString() + "    " + "Size:" + drive.TotalSize / (1024 * 1024 * 1024) + "MB" + Environment.NewLine;
                        }
                    }
                    Disk = details;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return Disk;
        }



        public string wifi()
        {
            string wifi = "";
            try
            {
                if (chkWifi.Checked)
                {
                    var availableNetwork = NativeWifi.EnumerateAvailableNetworks();

                    if (availableNetwork is null)
                        return "";

                    //await NativeWifi.ConnectNetworkAsync(interfaceId: availableNetwork.Interface.Id, profileName: availableNetwork.ProfileName, bssType: availableNetwork.BssType, timeout: TimeSpan.FromSeconds(10));

                    var connectedNetwork = NativeWifi.EnumerateConnectedNetworkSsids().FirstOrDefault();
                    var availableNetwork1 = NativeWifi.EnumerateAvailableNetworks().Where(x => x.Ssid.ToString() == connectedNetwork.ToString()).FirstOrDefault();

                    var connectedName = availableNetwork1?.ProfileName;

                    wifi = connectedName;
                }
            }
            catch (Exception ex)

            {
                MessageBox.Show("wifi"+ ex.Message);
            }
            return wifi;

        }


        public async void GetWeather()
        {
            
            try
            {
                if (chkWeather.Checked)
                {
                    label7.Text = latitude + "   lon" + longitute;
                    var httpClient = new HttpClient();
                    var ret = await httpClient.GetStringAsync($"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitute}&appid=8c81d78e7aad714087b48c351b120e99");
                    Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(ret);
                    weather = myDeserializedClass.weather[0].description + "  temp:" + (myDeserializedClass.main.temp).ToString() + "  feels_like:" + (myDeserializedClass.main.feels_like).ToString();
                   
                }
                else
                {
                    weather = "";
                }
                lblWeather.Text = weather;
            }
            catch(Exception ex)
            {
                MessageBox.Show("GetWeather"+ ex.Message);
            }
            
            

        }
        private void Watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e) // Find GeoLocation of Device  
        {
            try
            {
                if (e.Status == GeoPositionStatus.Ready)
                {
                    // Display the latitude and longitude.  
                    if (watcher.Position.Location.IsUnknown)
                    {
                        latitude = "0";
                        longitute = "0";
                    }
                    else
                    {
                        latitude = watcher.Position.Location.Latitude.ToString();
                        longitute = watcher.Position.Location.Longitude.ToString();
                        GetWeather();
                    }
                }
                else
                {
                    latitude = "0";
                    longitute = "0";
                }
            }
            catch (Exception)
            {
                latitude = "0";
                longitute = "0";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            watcher = new GeoCoordinateWatcher();
            watcher.StatusChanged += Watcher_StatusChanged;
            // Start the watcher.  
            watcher.Start();
            lblBattery.Text= Battery();
            lblOS.Text= os();
            lblDeviceName.Text= GetDiskspace();
            lblWifi.Text= wifi();
            GetWeather();
            //lblWeather.Text = weather;
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.UnscheduleJob(new TriggerKey("IDGJob", "IDG"));
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<Shedule>().Build();
            ITrigger trigger = TriggerBuilder.Create()
                          .WithIdentity("IDGJob", "IDG")
                          .WithSchedule(SimpleScheduleBuilder.RepeatMinutelyForever(min))
                          .StartAt(DateTime.Now)
                          .WithPriority(1)
                          .Build();
            scheduler.ScheduleJob(job, trigger);
            LoadGrid();
        }

        public void synccall()
        {
            try
            {
                string details = "";
                lblBattery.Text = Battery();
                lblOS.Text = os();
                lblDeviceName.Text = GetDiskspace();
                lblWifi.Text = wifi();
                GetWeather();
                SqlConnection con = new SqlConnection(OnConnection);
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();

                SqlCommand cmd = new SqlCommand("insert into details (Date,battery,device,wifi,storage,weather) values (getdate(),'" + lblBattery.Text +"','"+lblOS.Text+"','"+lblWifi.Text +"','"+lblDeviceName.Text + "','"+lblWeather.Text+"')", con);
                cmd.ExecuteNonQuery();
                con.Close();
                LoadGrid();
            }
            catch (Exception ex)
            {

            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string bat = "0";
                string os = "0";
                string wet = "0";
                string wifi = "0";
                string storage = "0";
                SqlConnection con = new SqlConnection(OnConnection);
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();

                if (chkBattery.Checked)
                {
                    bat = "1";
                }

                if (chkDevice.Checked )
                {
                    os = "1";
                }
                if (chkWifi.Checked)
                {
                   wifi = "1";
                }

                if (chkStorage.Checked)
                {
                    storage = "1";
                }
    
                if (chkWeather.Checked)
                {
                    wet = "1";
                }
                SqlCommand cmd = new SqlCommand("Update properties set status='"+ comboBoxinterval.SelectedValue.ToString() + "' where id = 1" +
                                               " Update properties set status='" + bat+ "' where id = 2" +
                                               " Update properties set status='" + os+ "' where id = 3" +
                                               " Update properties set status='" + wifi+ "' where id = 4 " +
                                               " Update properties set status='" + storage+ "' where id = 5 " +
                                               " Update properties set status='" + wet + "' where id = 6", con);
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Updated Successfully ");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Update Error :" + ex.Message);
            }
        }
    }
}
