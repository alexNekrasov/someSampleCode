/*
  COPYRIGHT © 2006 ESRI

  TRADE SECRETS: ESRI PROPRIETARY AND CONFIDENTIAL
  Unpublished material - all rights reserved under the
  Copyright Laws of the United States and applicable international
  laws, treaties, and conventions.
 
  For additional information, contact:
  Environmental Systems Research Institute, Inc.
  Attn: Contracts and Legal Services Department
  380 New York Street
  Redlands, California, 92373
  USA
 
  email: contracts@esri.com
*/

import java.awt.BorderLayout;
import java.io.FileFilter;
import java.io.File;
import java.io.IOException;
import javax.swing.JFileChooser;
import javax.swing.JPanel;
import javax.swing.JFrame;
import com.esri.arcgis.beans.toolbar.ToolbarBean;
import javax.swing.JSplitPane;
import com.esri.arcgis.beans.TOC.TOCBean;
import com.esri.arcgis.system.AoInitialize;
import com.esri.arcgis.system.EngineInitializer;
import com.esri.arcgis.system.esriLicenseProductCode;
import com.esri.arcgis.systemUI.esriCommandStyles;
import com.esri.arcgis.beans.map.MapBean;
import com.esri.arcgis.controls.ControlsMapFullExtentCommand;
import com.esri.arcgis.controls.ControlsMapPanTool;
import com.esri.arcgis.controls.ControlsMapZoomInTool;
import com.esri.arcgis.controls.ControlsMapZoomOutTool;

import javax.swing.filechooser.FileNameExtensionFilter;

public class BasicViewer extends JFrame {

	 public static void main(String[] args)throws Exception
	 {
	        //Step 1: Initialize the Java COM Interop.
	        EngineInitializer.initializeVisualBeans();

	        //Step 2: Initialize a valid License.
	        AoInitialize ao = new AoInitialize();
	        ao.initialize(esriLicenseProductCode.esriLicenseProductCodeEngine);
	        ao.checkOutExtension(com.esri.arcgis.system.esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst);

	        //Step 3: Create visual components for the mapviewer.

	        //Create a map visual component and load a .mxd map document.
	        MapBean map = new MapBean();
	        OpenRasters open = new OpenRasters();
	        
	        //FileChooser
	        JFileChooser fileChooser = new JFileChooser(".");
	        FileNameExtensionFilter filter = new FileNameExtensionFilter(
	                "Tiff Images", "tif");
	        fileChooser.setFileFilter(filter);
	        int returnVal = fileChooser.showOpenDialog(null);
	        if(returnVal == JFileChooser.APPROVE_OPTION) {
	        	map.addLayer(open.GetALayer(fileChooser.getSelectedFile().getName(), fileChooser.getSelectedFile().getParent()), 0);	        		    
	        }
	     
	        
	        //Create a toolbar visual component and add standard ESRI tools and commands.
	        ToolbarBean toolbar = new ToolbarBean();
	        toolbar.addItem(ControlsMapZoomInTool.getClsid(), 0, 0, false, 0,
	            esriCommandStyles.esriCommandStyleIconOnly);
	        toolbar.addItem(ControlsMapZoomOutTool.getClsid(), 0, 0, false, 0,
	            esriCommandStyles.esriCommandStyleIconOnly);
	        toolbar.addItem(new ControlsMapFullExtentCommand(), 0,  - 1, false, 0,
	            esriCommandStyles.esriCommandStyleIconOnly);
	        toolbar.addItem(ControlsMapPanTool.getClsid(), 0, 0, false, 0,
	            esriCommandStyles.esriCommandStyleIconOnly);

	        //Create a table of contents (TOCBean) visual component for the map.
	        TOCBean toc = new TOCBean();

	        //Buddy up the map component with the toolbar and TOC components.
	        toolbar.setBuddyControl(map);
	        toc.setBuddyControl(map);

	        //Step 4: Build the frame.
	        JFrame frame = new JFrame("Test Application");
	        frame.add(map, BorderLayout.CENTER);
	        frame.add(toolbar, BorderLayout.NORTH);
	        frame.add(toc, BorderLayout.WEST);
	        frame.setSize(1000, 1000);
	        frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
	        frame.setVisible(true);
	    }

}
