# FileOff.Tests

This project is a test project for the `.off` parse files.

## OFF Files

The conventional suffix for [OFF files](http://www.geomview.org/docs/html/OFF.html#OFF) is `.off`.

Syntax:

     [ST][C][N][4][n]OFF	# Header keyword
     [Ndim]		# Space dimension of vertices, present only if nOFF
     NVertices  NFaces  NEdges   # NEdges not used or checked
     
     x[0]  y[0]  z[0]	# Vertices, possibly with normals,
     			# colors, and/or texture coordinates, in that order,
     			# if the prefixes N, C, ST
     			# are present.
     			# If 4OFF, each vertex has 4 components,
     			# including a final homogeneous component.
     			# If nOFF, each vertex has Ndim components.
     			# If 4nOFF, each vertex has Ndim+1 components.
     ...
     x[NVertices-1]  y[NVertices-1]  z[NVertices-1]
     
         			# Faces
         			# Nv = # vertices on this face
         			# v[0] ... v[Nv-1]: vertex indices
         			#		in range 0..NVertices-1
     Nv  v[0] v[1] ... v[Nv-1]  colorspec
     ...
         			# colorspec continues past v[Nv-1]
         			# to end-of-line; may be 0 to 4 numbers
         			# nothing: default
         			# integer: colormap index
         			# 3 or 4 integers: RGB[A] values 0..255
     			# 3 or 4 floats: RGB[A] values 0..1

OFF files (name for "object file format") represent collections of planar polygons with possibly shared vertices, a convenient way to describe polyhedra. The polygons may be concave but there's no provision for polygons containing holes. 

* [Reference: OFF files](http://www.geomview.org/docs/html/OFF.html#OFF)

### References

* [Wikipedia - OFF (file format)](https://en.wikipedia.org/wiki/OFF_(file_format))
* [fsu.edu - OFF Files](https://people.sc.fsu.edu/~jburkardt/data/off/off.html)
* [holmes3d.net - OFF Files](http://www.holmes3d.net/graphics/offfiles/)
* [3DViewer](https://3dviewer.net/index.html#model=assets/models/cube.off)
