
How to create "YAT.res"
-----------------------

1.	Install "Gimp"
	<http://www.gimp.org/windows/>

2.	Install "Resource Hacker"
	<http://angusj.com/resourcehacker/>

3.	Take the "Nuvola" <.png> files

4.	Add modified "Nuvola" <.png> files where needed

5.	Use "Gimp" to merge the "Nuvola" <.png> files into YAT images
	"Gimp" files <YAT_*_wxh.xcf> => <YAT/YAT_*_wxh.png>

6.	Use "Gimp" to create 8-bpp palette images
	<YAT/YAT_*_wxh.png> => <YAT_8bpp/YAT_*_wxh.png>

7.	In "Gimp", create multi-layered pictures that contain one image on each layer
	<YAT.xcf>, <YAT_*.xcf>
	Images must be ordered
	- 8bpp
	  -  48x48
	  -  32x32
	  -  16x16
	  -  64x64
	  - 128x128
	- 32bpp
	  -  48x48
	  -  32x32
	  -  16x16
	  -  64x64
	  - 128x128

8.	Save pictures as <.ico>

9.	Use "Resource Hacker" to add the icons to <YAT.res>
	- Resources must be named "1", "2",...


---

Matthias Klaey, 2008-02-05
