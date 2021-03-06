﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace r2pipe_test
{
    public class GuiControl
    {
        public object control   = null;
        public string name      = null;
        public string sName     = null;
        public string cmds      = null;
        public string tabTitle  = null;
        public string pre_cmd   = null;
        public string pos_cmd   = null;
        public bool   closed    = false;
        public bool   top_level = true;
        public int    tab_index = -1;
        public bool synchronize = true;
        public string address_tag = null;
        public webbrowser_container_form wcf = null;
        public List<string> column_titles = null;
        public GuiControl(object control, string name, string sName, string cmds, string tabTitle, 
            List<string> column_titles = null, string pre_cmd = null, string pos_cmd=null, int tab_index = -1,
            webbrowser_container_form wcf = null)
        {
            this.control    = control;
            this.name       = name;
            this.sName      = sName;
            this.cmds       = cmds;
            this.tabTitle   = tabTitle;
            this.column_titles = column_titles;
            this.pre_cmd    = pre_cmd;
            this.pos_cmd    = pos_cmd;
            this.tab_index  = tab_index;
            this.wcf        = wcf;
        }
        public void set_columnTitles(List<string> column_titles)
        {
            this.column_titles = column_titles;
        }
        /*
        public override string ToString()
        {            
            return string.Format(
                "control  : {0}\n" +
                "name     : {1}\n" +
                "cmds     : {2}\n" +
                "tabTitle : {3}\n" +
                "column_titles : {4}",
                control.ToString(), // access problems
                name, cmds, tabTitle,
                column_titles.ToString());
        }*/
    }
    public class GuiControls
    {
        private R2PIPE_WRAPPER r2pw       = null;
        public  List<GuiControl> controls = null;
        public GuiControls(R2PIPE_WRAPPER r2pw)
        {
            this.r2pw = r2pw;
            controls = new List<GuiControl>();
        }
        public GuiControl add_control(string name, object control, string tabTitle = null, string cmds = null,
        string pre_cmd = null, string pos_cmd = null, int tab_index = -1,
            webbrowser_container_form wcf = null)
        {
            GuiControl gui_control = null;
            if (name == null || name.Length == 0) return null;
            /*
            if (control == null)
            {
                Console.WriteLine("warning: GuiControl: add_control(): control name='" + name + "' is null", "control is null");
                //return null;
            }*/
            gui_control = findControlBy_name(name);
            if (gui_control != null)
            {
                gui_control.closed = false;
                gui_control.cmds = cmds;
                return gui_control;
            }
            try
            {
                string sName = name;
                if( sName.Contains("_") )
                {
                    int pos = sName.IndexOf("_");
                    sName = sName.Substring(0, pos);
                }
                gui_control = new GuiControl(
                    control, name, sName, cmds, tabTitle, null,
                    pre_cmd, pos_cmd, tab_index, wcf);
                controls.Add(gui_control);
            }
            catch (Exception e)
            {
                r2pw.Show(e.ToString(), "GuiControls: add_control()");
            }
            return gui_control;
        }
        public void dump(string dummy=null)
        {
            foreach (GuiControl c in controls)
            {
                string ctype = "";
                if (c.control == null) continue;
                r2pw.output(string.Format("{0,12}|{1,12}|{2,20}|{3}",
                    c.tabTitle, c.cmds, c.name, ctype));
            }
        }
        public GuiControl findControlBy_name(string name, int skip=0)
        {
            if (name == null) return null;
            foreach (GuiControl c in controls)
            {
                if (name.Equals(c.name))
                {
                    if (skip-- > 0) continue; 
                    return c;
                }
            }
            return null;
        }
        public GuiControl findControlBy_cmds(string cmds)
        {
            foreach (GuiControl c in controls)
            {
                if (cmds.Equals(c.cmds))
                    return c;
            }
            return null;
        }
        public GuiControl findControlBy_tabTitle(string tabTitle)
        {
            if (tabTitle == null) return null;
            foreach (GuiControl c in controls)
            {
                if (c.tabTitle!=null && c.tabTitle.StartsWith(tabTitle)) 
                    return c;
            }
            return null;
        }
        public void remove_control_byName(string controlName, int skip=0)
        {
            GuiControl c = findControlBy_name(controlName, skip);
            while (c != null)
            {
                controls.Remove(c);
                c = findControlBy_name(controlName, skip);
            }
        }
        public void close_control_byName(string controlName)
        {
            GuiControl c = findControlBy_name(controlName);
            int skip = 0;
            while (c != null)
            {
                c.closed = true;
                c = findControlBy_name(controlName, ++skip);
            }
        }
        public GuiControl get_active_control()
        {
            string tabTitle = r2pw.guicontrol.selected_tab("title");
            GuiControl gui_control = findControlBy_tabTitle(tabTitle);
            return gui_control;
        }
    }
}
