# Layers

This project was developed during my research project and master thesis at HTW Berlin and allows users to create abstract art with virtual oil paint. The simulation is based on the concept of bidirectional paint transfer but extends previous approaches for applicability to working with a special kind of squeegee. Key improvements are:
+ Support for arbitrarily many layers of paint
+ New, time-independent model for calculating the amounts of transferred paint
+ More precise imprint calculation
+ Advanced interaction model, so a user can interact with varying layers of paint within a single stroke

The simulation itself is mostly implemented in HLSL Compute Shaders to provide scalable performance.

## Results
<img src="./readme_data/13_15294f6c.png" width=80%/>
<hr width=80%>
<img src="./readme_data/13_15294f6c_2.png" width=80%/>
<hr width=80%>
<img src="./readme_data/21_c70d1466a.png" width=80%/>
<hr width=80%>
<img src="./readme_data/34_15f5e446_RCV_512.png" width=80%/>
<hr width=80%>
<img src="./readme_data/30_f06498d8_1.png" width=60%/>
<hr width=80%>
<img src="./readme_data/30_f06498d8_2.png" width=60%/>
<hr width=80%>
<img src="./readme_data/34_15f5e446_2.png" width=80%/>
<hr width=80%>
<img src="./readme_data/34_15f5e446_8.png" width=80%/>
<hr width=80%>


## Demo
<img src="./readme_data/demo.gif" width=80%/>