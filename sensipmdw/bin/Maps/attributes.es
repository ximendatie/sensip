var root = document.documentElement;
var anim = root.getElementById('anim');
var currentNode = root.getElementById('start');

function zoomto(eltToZoom) {
		var newBox = printViewBox(document.getElementById(eltToZoom));
		var currentBox = printViewBox(currentNode);
		
		anim.setAttribute('values', currentBox + ';' + newBox);
		currentNode = document.getElementById(eltToZoom);
		anim.beginElement();
}

function hover(eltName, textEltName) {
	var element = document.getElementById(eltName);
	var hud = root.getElementById('HUD');
	var x = element.getAttribute('cx');
	var y = element.getAttribute('cy');
	
	hud.setAttribute('x', x);
	hud.setAttribute('y', y);
	hud.setAttribute('height', '60');
	hud.setAttribute('width', '250');
	
	var username = document.getElementById(textEltName);
	var hudText = root.getElementById('HUDText');
	hudText.replaceChild(username.cloneNode(true).getFirstChild(), hudText.getFirstChild());
	hudText.setAttribute('x', (parseFloat(x) + 125));
	hudText.setAttribute('y', (parseInt(y) + 40));
	hudText.setAttribute('font-size', 28);
}

function hoverout() {
	var hud = root.getElementById('HUD');
	
	hud.setAttribute('x', '0');
	hud.setAttribute('y', '0');
	hud.setAttribute('height', '0');
	hud.setAttribute('width', '0');
	
	var hudText = root.getElementById('HUDText');
	var space = document.createTextNode(" ");
	hudText.replaceChild(space, hudText.getFirstChild());
}

function drawLocDot(){
	var nullCircle = root.getElementById('nullCircle');
	var newDot = nullCircle.cloneElement(false);

	newDot.setAttribute('x', x);
	newDot.setAttribute('y', y);
	newDot.setAttribute('r', r);
	newDot.setAttribute('id', id);
	newDot.setAttribute('style', style);
	
	root.addElement(newDot);
}

function printViewBox (node) {
	if (node.id != 'start') {
		var box = getBoundingBox(node);
		return (box.x) + ' ' + (box.y-80) + ' ' + (box.width + 120) + ' ' + (box.height + 120);
	} 
	else {
		return root.getAttribute('viewBox');
	}
}

function highlight(eltName) {	
	var mouseOverBox = document.getElementById('mouseoverBox');

	var e = document.getElementById(eltName);
	var x = e.getAttribute('x');
	var y = e.getAttribute('y');
	var width = e.getAttribute('width');
	var height = e.getAttribute('height');
	
	mouseOverBox.setAttribute('x', x);
	mouseOverBox.setAttribute('y', y);
	mouseOverBox.setAttribute('width', width);
	mouseOverBox.setAttribute('height', height);
}

function unHighlight(eltName) {
	var mouseOverBox = document.getElementById('mouseoverBox');
	mouseOverBox.setAttribute('x', 0);
	mouseOverBox.setAttribute('y', 0);
	mouseOverBox.setAttribute('width', 0);
	mouseOverBox.setAttribute('height', 0);
}
