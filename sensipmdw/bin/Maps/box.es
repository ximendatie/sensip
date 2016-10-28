// takes a node as parameter and returns its
// bounding box as an SVGRect
/////////////////////////////
function getBoundingBox(node) {
  var root = document.documentElement;
  var matrix = getTransformToElement(node, root);
  var box = node.getBBox();
  var corners = new Array();
  var point = root.createSVGPoint();
  point.x = box.x;
  point.y = box.y;
  corners.push( point.matrixTransform(matrix) );
  point.x = box.x + box.width;
  point.y = box.y;
  corners.push( point.matrixTransform(matrix) );
  point.x = box.x + box.width;
  point.y = box.y + box.height;
  corners.push( point.matrixTransform(matrix) );
  point.x = box.x;
  point.y = box.y + box.height;
  corners.push( point.matrixTransform(matrix) );
  var minX = corners[0].x;
  var maxX = corners[0].x;
  var minY = corners[0].y;
  var maxY = corners[0].y;
  for (var i = 1; i < corners.length; i++) {
    var x = corners[i].x;
    var y = corners[i].y;
    if (x < minX) {
      minX = x;
    } else if (x > maxX) {
      maxX = x;
    }
    if (y < minY) {
      minY = y;
    } else if (y > maxY) {
      maxY = y;
    }
  }
  box.x = minX;
  box.y = minY;
  box.width = maxX - minX;
  box.height = maxY - minY;
  return box;
}

// returns an SVGMatrix representing the inherited
// transformations of an element in respect to the given target
///////////////////////////////////////////////////////////////
function getTransformToElement(node, target) {
  var ctm = node.getCTM();
  var node = node.parentNode;
  while ( node != target ) {
    ctm  = node.getCTM().multiply(ctm);
    node = node.parentNode;
  }
  return ctm;
}

