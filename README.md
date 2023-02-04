# RE4-PS2-BIN-TOOL
Extract and repack ps2 re4 bin files

Translate from Portuguese Brazil

Esse é o código resultado a partir das pesquisa sobre os arquivos .bin do re4 de ps2 em conjunto com o HardRain.
Encontra-se em estagio de desenvolvimento, mas já pode ser usado para criar novos .bin para os cenários e os itens.

**Codigo de terceiro:**

[ObjLoader by chrisjansson](https://github.com/chrisjansson/ObjLoader/).
Encontra-se no BINrepackTest, codigo modificado, não consireda mais a tag "g", os grupos são criados a partir da tag de material "usemtl" 

**BINdecoderTest**

Decodifica e cria um .obj do bin.
São criados os arquivos .txt2 .obj .mtl e .idxbin
  - sendo os arquivos txt2 arquivos informativos que divide o bin por partes para melhor visualização.
  - .OBJ: caso o campo "IndexMount" tenha a sequencia e 0x0, 0x1 e 0xFFFF, o arquivo estará correto, caso seja sempre zero estará errado pois o programa não sabe como montar as vertices desse tipo de arquivo
  - .MTL: arquivo que acompanha o .obj serve para carregar as texturas nos modelos, porem as texturas devem ser extraidas do .TPL, e enumeradas a partir de zero.
  - .IDXBIN: é o arquivo que contem o conteudo que não esta no .obj mas é necessario para recompilar o .BIN
  
**BINrepackTest**

Cria um novo arquivo .BIN a partir do conteudo do .IDXBIN e .OBJ
  - A quantidade de materias do bin é definido pelo arquivo .obj, os materias seram colocados em ordem alfabetica no arquivo bin.
  - por exemplo: caso o arquivo obj tenha 4 materais, no arquivo .idxbin deve conter 4 entradas cada com as tags "materialLine_*", "NodeHeaderLineP2_Length_*" e "NodeHeaderLineP2_*", sendo * um numero a partir de 0.
  
  
