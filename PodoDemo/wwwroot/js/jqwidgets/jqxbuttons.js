/*
jQWidgets v4.5.1 (2017-April)
Copyright (c) 2011-2017 jQWidgets.
License: http://jqwidgets.com/license/
*/
!function(a){a.jqx.cssroundedcorners=function(a){var b={all:"jqx-rc-all",top:"jqx-rc-t",bottom:"jqx-rc-b",left:"jqx-rc-l",right:"jqx-rc-r","top-right":"jqx-rc-tr","top-left":"jqx-rc-tl","bottom-right":"jqx-rc-br","bottom-left":"jqx-rc-bl"};for(prop in b)if(b.hasOwnProperty(prop)&&a==prop)return b[prop]},a.jqx.jqxWidget("jqxButton","",{}),a.extend(a.jqx._jqxButton.prototype,{defineInstance:function(){var b={type:"",cursor:"arrow",roundedCorners:"all",disabled:!1,height:null,width:null,overrideTheme:!1,enableHover:!0,enableDefault:!0,enablePressed:!0,imgPosition:"center",imgSrc:"",imgWidth:16,imgHeight:16,value:null,textPosition:"",textImageRelation:"overlay",rtl:!1,_ariaDisabled:!1,_scrollAreaButton:!1,template:"default",aria:{"aria-disabled":{name:"disabled",type:"boolean"}}};return this===a.jqx._jqxButton.prototype?b:(a.extend(!0,this,b),b)},_addImage:function(b){var c=this;if("input"==c.element.nodeName.toLowerCase()||"button"==c.element.nodeName.toLowerCase()||"div"==c.element.nodeName.toLowerCase()){if(c._img)c._img.setAttribute("src",c.imgSrc),c._img.setAttribute("width",c.imgWidth),c._img.setAttribute("height",c.imgHeight),c._text.innerHTML=c.value;else{c.field=c.element,c.field.className&&(c._className=c.field.className);var d={title:c.field.title},e=null;if(c.field.getAttribute("value"))var e=c.field.getAttribute("value");else if("input"!=c.element.nodeName.toLowerCase())var e=c.element.innerHTML;c.value&&(e=c.value),c.field.id.length?d.id=c.field.id.replace(/[^\w]/g,"_")+"_"+b:d.id=a.jqx.utilities.createId()+"_"+b;var f=document.createElement("div");f.id=d.id,f.title=d.title,f.style.cssText=c.field.style.cssText,f.style.boxSizing="border-box";var g=document.createElement("img");g.setAttribute("src",c.imgSrc),g.setAttribute("width",c.imgWidth),g.setAttribute("height",c.imgHeight),f.appendChild(g),c._img=g;var h=document.createElement("span");e&&(h.innerHTML=e,c.value=e),f.appendChild(h),c._text=h,c.field.style.display="none",c.field.parentNode&&c.field.parentNode.insertBefore(f,c.field.nextSibling);var i=c.host.data();c.host=a(f),c.host.data(i),c.element=f,c.element.id=c.field.id,c.field.id=d.id;var j=new jqxHelper(c.element),k=new jqxHelper(c.field);if(c._className&&(j.addClass(c._className),k.removeClass(c._className)),c.field.tabIndex){var l=c.field.tabIndex;c.field.tabIndex=-1,c.element.tabIndex=l}}c.imgSrc?c._img.style.display="inline":c._img.style.display="none",c.value?c._text.style.display="inline":c._text.style.display="none",c._positionTextAndImage()}},_positionTextAndImage:function(){var a=this,b=a.element.offsetWidth,c=a.element.offsetHeight,d=a.imgWidth,e=a.imgHeight;""==a.imgSrc&&(d=0,e=0);var f=a._text.offsetWidth,g=a._text.offsetHeight,h=4,i=4,j=4,k=0,l=0;switch(a.textImageRelation){case"imageBeforeText":case"textBeforeImage":k=d+f+2*j+h+2*i,l=Math.max(e,g)+2*j+h+2*i;break;case"imageAboveText":case"textAboveImage":k=Math.max(d,f)+2*j,l=e+g+h+2*j+2*i;break;case"overlay":k=Math.max(d,f)+2*j,l=Math.max(e,g)+2*j}a.width||(a.element.style.width=k+"px",b=k),a.height||(a.element.style.height=l+"px",c=l),a._img.style.position="absolute",a._text.style.position="absolute",a.element.style.position="relative",a.element.style.overflow="hidden";var m={},n={},o=function(a,b,c,d,e){switch(b.width<d&&(b.width=d),b.height<e&&(b.height=e),c){case"left":a.style.left=b.left+"px",a.style.top=b.top+b.height/2-e/2+"px";break;case"topLeft":a.style.left=b.left+"px",a.style.top=b.top+"px";break;case"bottomLeft":a.style.left=b.left+"px",a.style.top=b.top+b.height-e+"px";break;default:case"center":a.style.left=b.left+b.width/2-d/2+"px",a.style.top=b.top+b.height/2-e/2+"px";break;case"top":a.style.left=b.left+b.width/2-d/2+"px",a.style.top=b.top+"px";break;case"bottom":a.style.left=b.left+b.width/2-d/2+"px",a.style.top=b.top+b.height-e+"px";break;case"right":a.style.left=b.left+b.width-d+"px",a.style.top=b.top+b.height/2-e/2+"px";break;case"topRight":a.style.left=b.left+b.width-d+"px",a.style.top=b.top+"px";break;case"bottomRight":a.style.left=b.left+b.width-d+"px",a.style.top=b.top+b.height-e+"px"}},p=0,q=0,r=b,s=c,t=(r-p)/2,u=(s-q)/2,v=a._img,w=a._text,x=s-q,y=r-p;switch(p+=i,q+=i,r=r-i-2,y=y-2*i-2,x=x-2*i-2,a.textImageRelation){case"imageBeforeText":switch(a.imgPosition){case"left":case"topLeft":case"bottomLeft":n={left:p,top:q,width:p+d,height:x},m={left:p+d+h,top:q,width:y-d-h,height:x};break;case"center":case"top":case"bottom":n={left:t-f/2-d/2-h/2,top:q,width:d,height:x},m={left:n.left+d+h,top:q,width:r-n.left-d-h,height:x};break;case"right":case"topRight":case"bottomRight":n={left:r-f-d-h,top:q,width:d,height:x},m={left:n.left+d+h,top:q,width:r-n.left-d-h,height:x}}o(v,n,a.imgPosition,d,e),o(w,m,a.textPosition,f,g);break;case"textBeforeImage":switch(a.textPosition){case"left":case"topLeft":case"bottomLeft":m={left:p,top:q,width:p+f,height:x},n={left:p+f+h,top:q,width:y-f-h,height:x};break;case"center":case"top":case"bottom":m={left:t-f/2-d/2-h/2,top:q,width:f,height:x},n={left:m.left+f+h,top:q,width:r-m.left-f-h,height:x};break;case"right":case"topRight":case"bottomRight":m={left:r-f-d-h,top:q,width:f,height:x},n={left:m.left+f+h,top:q,width:r-m.left-f-h,height:x}}o(v,n,a.imgPosition,d,e),o(w,m,a.textPosition,f,g);break;case"imageAboveText":switch(a.imgPosition){case"topRight":case"top":case"topLeft":n={left:p,top:q,width:y,height:e},m={left:p,top:q+e+h,width:y,height:x-e-h};break;case"left":case"center":case"right":n={left:p,top:u-e/2-g/2-h/2,width:y,height:e},m={left:p,top:n.top+h+e,width:y,height:x-n.top-h-e};break;case"bottomLeft":case"bottom":case"bottomRight":n={left:p,top:s-e-g-h,width:y,height:e},m={left:p,top:n.top+h+e,width:y,height:g}}o(v,n,a.imgPosition,d,e),o(w,m,a.textPosition,f,g);break;case"textAboveImage":switch(a.textPosition){case"topRight":case"top":case"topLeft":m={left:p,top:q,width:y,height:g},n={left:p,top:q+g+h,width:y,height:x-g-h};break;case"left":case"center":case"right":m={left:p,top:u-e/2-g/2-h/2,width:y,height:g},n={left:p,top:m.top+h+g,width:y,height:x-m.top-h-g};break;case"bottomLeft":case"bottom":case"bottomRight":m={left:p,top:s-e-g-h,width:y,height:g},n={left:p,top:m.top+h+g,width:y,height:e}}o(v,n,a.imgPosition,d,e),o(w,m,a.textPosition,f,g);break;case"overlay":default:m={left:p,top:q,width:y,height:x},n={left:p,top:q,width:y,height:x},o(v,n,a.imgPosition,d,e),o(w,m,a.textPosition,f,g)}},createInstance:function(b){var c=this;c._setSize(),c.buttonObj=new jqxHelper(c.element),(""!=c.imgSrc||""!=c.textPosition||c.element.value&&c.element.value.indexOf("<")>=0||null!=c.value)&&(c.refresh(),c._addImage("jqxButton"),c.buttonObj=new jqxHelper(c.element)),c._ariaDisabled||c.element.setAttribute("role","button"),""!==c.type&&c.element.setAttribute("type",c.type),c.overrideTheme||(c.buttonObj.addClass(c.toThemeProperty(a.jqx.cssroundedcorners(c.roundedCorners))),c.enableDefault&&c.buttonObj.addClass(c.toThemeProperty("jqx-button")),c.buttonObj.addClass(c.toThemeProperty("jqx-widget"))),c.isTouchDevice=a.jqx.mobile.isTouchDevice(),c._ariaDisabled||a.jqx.aria(this),"arrow"!=c.cursor&&(c.disabled?c.element.style.cursor="arrow":c.element.style.cursor=c.cursor);var d="mouseenter mouseleave mousedown focus blur";if(c._scrollAreaButton)var d="mousedown";c.isTouchDevice&&(c.addHandler(c.host,a.jqx.mobile.getTouchEventName("touchstart"),function(a){c.isPressed=!0,c.refresh()}),c.addHandler(a(document),a.jqx.mobile.getTouchEventName("touchend")+"."+c.element.id,function(a){c.isPressed=!1,c.refresh()})),c.addHandler(c.host,d,function(a){switch(a.type){case"mouseenter":c.isTouchDevice||!c.disabled&&c.enableHover&&(c.isMouseOver=!0,c.refresh());break;case"mouseleave":c.isTouchDevice||!c.disabled&&c.enableHover&&(c.isMouseOver=!1,c.refresh());break;case"mousedown":c.disabled||(c.isPressed=!0,c.refresh());break;case"focus":c.disabled||(c.isFocused=!0,c.refresh());break;case"blur":c.disabled||(c.isFocused=!1,c.refresh())}}),c.mouseupfunc=function(a){c.disabled||(c.isPressed||c.isMouseOver)&&(c.isPressed=!1,c.refresh())},c.addHandler(document,"mouseup.button"+c.element.id,c.mouseupfunc);try{if((""!=document.referrer||window.frameElement)&&null!=window.top&&window.top!=window.that){var e="";if(window.parent&&document.referrer&&(e=document.referrer),e.indexOf(document.location.host)!=-1){var f=function(a){c.isPressed=!1,c.refresh()};window.top.document&&c.addHandler(a(window.top.document),"mouseup",f)}}}catch(a){}c.propertyChangeMap.roundedCorners=function(b,c,d,e){b.buttonObj.removeClass(b.toThemeProperty(a.jqx.cssroundedcorners(d))),b.buttonObj.addClass(b.toThemeProperty(a.jqx.cssroundedcorners(e)))},c.propertyChangeMap.disabled=function(b,c,d,e){d!=e&&(b.refresh(),b.element.setAttribute("disabled",e),b.element.disabled=e,e?b.element.style.cursor="default":b.element.style.cursor=b.cursor,a.jqx.aria(b,"aria-disabled",b.disabled))},c.propertyChangeMap.rtl=function(a,b,c,d){c!=d&&a.refresh()},c.propertyChangeMap.template=function(a,b,c,d){c!=d&&(a.buttonObj.removeClass(a.toThemeProperty("jqx-"+c)),a.refresh())},c.propertyChangeMap.theme=function(b,c,d,e){b.buttonObj.removeClass(b.element),b.enableDefault&&b.buttonObj.addClass(b.toThemeProperty("jqx-button")),b.buttonObj.addClass(b.toThemeProperty("jqx-widget")),b.overrideTheme||b.buttonObj.addClass(b.toThemeProperty(a.jqx.cssroundedcorners(b.roundedCorners))),b._oldCSSCurrent=null,b.refresh()},c.disabled&&(c.element.disabled=!0,c.element.setAttribute("disabled","true"))},resize:function(a,b){this.width=a,this.height=b,this._setSize()},val:function(){var b=this,c=b.host.find("input");return c.length>0?0==arguments.length||"object"==typeof value?c.val():(c.val(value),b.refresh(),c.val()):0==arguments.length||"object"==typeof value?"button"==b.element.nodeName.toLowerCase()?a(b.element).text():b.element.value:(b.element.value=arguments[0],"button"==b.element.nodeName.toLowerCase()&&a(b.element).text(arguments[0]),void b.refresh())},_setSize:function(){var a=this,b=a.height,c=a.width;b&&(isNaN(b)||(b+="px"),a.element.style.height=b),c&&(isNaN(c)||(c+="px"),a.element.style.width=c)},_removeHandlers:function(){var b=this;b.removeHandler(b.host,"selectstart"),b.removeHandler(b.host,"click"),b.removeHandler(b.host,"focus"),b.removeHandler(b.host,"blur"),b.removeHandler(b.host,"mouseenter"),b.removeHandler(b.host,"mouseleave"),b.removeHandler(b.host,"mousedown"),b.removeHandler(a(document),"mouseup.button"+b.element.id,b.mouseupfunc),b.isTouchDevice&&(b.removeHandler(b.host,a.jqx.mobile.getTouchEventName("touchstart")),b.removeHandler(a(document),a.jqx.mobile.getTouchEventName("touchend")+"."+b.element.id)),b.mouseupfunc=null,delete b.mouseupfunc},focus:function(){this.host.focus()},destroy:function(){var b=this;b._removeHandlers();var c=a.data(b.element,"jqxButton");c&&delete c.instance,b.host.removeClass(),b.host.removeData(),b.host.remove(),delete b.set,delete b.get,delete b.call,delete b.element,delete b.host},render:function(){this.refresh()},propertiesChangedHandler:function(a,b,c){c&&c.width&&c.height&&2==Object.keys(c).length&&(a._setSize(),a.refresh())},propertyChangedHandler:function(a,b,c,d){void 0!=this.isInitialized&&0!=this.isInitialized&&d!=c&&(a.batchUpdate&&a.batchUpdate.width&&a.batchUpdate.height&&2==Object.keys(a.batchUpdate).length||("type"===b&&a.element.setAttribute("type",d),"textImageRelation"!=b&&"textPosition"!=b&&"imgPosition"!=b||(a._img?a._positionTextAndImage():a._addImage("jqxButton")),"imgSrc"!=b&&"imgWidth"!=b&&"imgHeight"!=b&&"value"!=b||a._addImage("jqxButton"),"width"!=b&&"height"!=b||(a._setSize(),a.refresh())))},refresh:function(){var a=this;if(!a.overrideTheme){var b=a.toThemeProperty("jqx-fill-state-focus"),c=a.toThemeProperty("jqx-fill-state-disabled"),d=a.toThemeProperty("jqx-fill-state-normal");a.enableDefault||(d="");var e=a.toThemeProperty("jqx-fill-state-hover"),f=a.toThemeProperty("jqx-fill-state-pressed"),g=a.toThemeProperty("jqx-fill-state-pressed");a.enablePressed||(f="");var h="";if(a.host){if(a.element.disabled=a.disabled,a.disabled)return a._oldCSSCurrent&&a.buttonObj.removeClass(a._oldCSSCurrent),h=d+" "+c,"default"!==a.template&&""!==a.template&&(h+=" jqx-"+a.template,""!=a.theme&&(h+=" jqx-"+a.template+"-"+a.theme)),a.buttonObj.addClass(h),void(a._oldCSSCurrent=h);h=a.isMouseOver&&!a.isTouchDevice?a.isPressed?g:e:a.isPressed?f:d,a.isFocused&&(h+=" "+b),"default"!==a.template&&""!==a.template&&(h+=" jqx-"+a.template,""!=a.theme&&(h+=" jqx-"+a.template+"-"+a.theme)),h!=a._oldCSSCurrent&&(a._oldCSSCurrent&&a.buttonObj.removeClass(a._oldCSSCurrent),a.buttonObj.addClass(h),a._oldCSSCurrent=h),a.rtl&&(a.buttonObj.addClass(a.toThemeProperty("jqx-rtl")),a.element.style.direction="rtl")}}}}),a.jqx.jqxWidget("jqxLinkButton","",{}),a.extend(a.jqx._jqxLinkButton.prototype,{defineInstance:function(){this.disabled=!1,this.height=null,this.width=null,this.rtl=!1,this.href=null},createInstance:function(b){var c=this;this.host.onselectstart=function(){return!1},this.host.attr("role","button");var d=this.height||this.element.offsetHeight,e=this.width||this.element.offsetWidth;this.href=this.element.getAttribute("href"),this.target=this.element.getAttribute("target"),this.content=this.host.text(),this.element.innerHTML="";var f=document.createElement("input");f.type="button",f.className="jqx-wrapper "+this.toThemeProperty("jqx-reset"),this._setSize(f,e,d),f.value=this.content;var g=new jqxHelper(this.element);g.addClass(this.toThemeProperty("jqx-link")),this.element.style.color="inherit",this.element.appendChild(f),this._setSize(f,e,d);var h=void 0==b?{}:b[0]||{};a(f).jqxButton(h),this.wrapElement=f,this.disabled&&(this.element.disabled=!0),this.propertyChangeMap.disabled=function(a,b,c,d){a.element.disabled=d,a.wrapElement.jqxButton({disabled:d})},this.addHandler(a(f),"click",function(a){return this.disabled||c.onclick(a),!1})},_setSize:function(a,b,c){c&&(isNaN(c)||(c+="px"),a.style.height=c),b&&(isNaN(b)||(b+="px"),a.style.width=b)},onclick:function(a){null!=this.target?window.open(this.href,this.target):window.location=this.href}}),a.jqx.jqxWidget("jqxRepeatButton","jqxButton",{}),a.extend(a.jqx._jqxRepeatButton.prototype,{defineInstance:function(){this.delay=50},createInstance:function(b){var c=this,d=a.jqx.mobile.isTouchDevice(),e=d?"touchend."+this.base.element.id:"mouseup."+this.base.element.id,f=d?"touchstart."+this.base.element.id:"mousedown."+this.base.element.id;this.addHandler(a(document),e,function(a){null!=c.timeout&&(clearTimeout(c.timeout),c.timeout=null,c.refresh()),void 0!=c.timer&&(clearInterval(c.timer),c.timer=null,c.refresh())}),this.addHandler(this.base.host,f,function(a){null!=c.timer&&clearInterval(c.timer),c.timeout=setTimeout(function(){clearInterval(c.timer),c.timer=setInterval(function(a){c.ontimer(a)},c.delay)},150)}),this.mousemovefunc=function(a){d||0==a.which&&null!=c.timer&&(clearInterval(c.timer),c.timer=null)},this.addHandler(this.base.host,"mousemove",this.mousemovefunc)},destroy:function(){var b=a.jqx.mobile.isTouchDevice(),c=b?"touchend."+this.base.element.id:"mouseup."+this.base.element.id,d=b?"touchstart."+this.base.element.id:"mousedown."+this.base.element.id;this.removeHandler(this.base.host,"mousemove",this.mousemovefunc),this.removeHandler(this.base.host,d),this.removeHandler(a(document),c),this.timer=null,delete this.mousemovefunc,delete this.timer;var e=a.data(this.base.element,"jqxRepeatButton");e&&delete e.instance,a(this.base.element).removeData(),this.base.destroy(),delete this.base},stop:function(){clearInterval(this.timer),this.timer=null},ontimer:function(b){var b=new a.Event("click");null!=this.base&&null!=this.base.host&&this.base.host.trigger(b)}}),a.jqx.jqxWidget("jqxToggleButton","jqxButton",{}),a.extend(a.jqx._jqxToggleButton.prototype,{defineInstance:function(){this.toggled=!1,this.uiToggle=!0,this.aria={"aria-checked":{name:"toggled",type:"boolean"},"aria-disabled":{name:"disabled",type:"boolean"}}},createInstance:function(b){var c=this;c.base.overrideTheme=!0,c.isTouchDevice=a.jqx.mobile.isTouchDevice(),a.jqx.aria(this),c.propertyChangeMap.roundedCorners=function(b,c,d,e){b.base.buttonObj.removeClass(b.toThemeProperty(a.jqx.cssroundedcorners(d))),b.base.buttonObj.addClass(b.toThemeProperty(a.jqx.cssroundedcorners(e)))},c.propertyChangeMap.toggled=function(a,b,c,d){a.refresh()},c.propertyChangeMap.disabled=function(a,b,c,d){a.base.disabled=d,a.refresh()},c.addHandler(c.base.host,"click",function(a){!c.base.disabled&&c.uiToggle&&c.toggle()}),c.isTouchDevice||(c.addHandler(c.base.host,"mouseenter",function(a){c.base.disabled||c.refresh()}),c.addHandler(c.base.host,"mouseleave",function(a){c.base.disabled||c.refresh()})),c.addHandler(c.base.host,"mousedown",function(a){c.base.disabled||c.refresh()}),c.addHandler(a(document),"mouseup.togglebutton"+c.base.element.id,function(a){c.base.disabled||c.refresh()})},destroy:function(){this._removeHandlers(),this.base.destroy()},_removeHandlers:function(){this.removeHandler(this.base.host,"click"),this.removeHandler(this.base.host,"mouseenter"),this.removeHandler(this.base.host,"mouseleave"),this.removeHandler(this.base.host,"mousedown"),this.removeHandler(a(document),"mouseup.togglebutton"+this.base.element.id)},toggle:function(){this.toggled=!this.toggled,this.refresh(),a.jqx.aria(this,"aria-checked",this.toggled)},unCheck:function(){this.toggled=!1,this.refresh()},check:function(){this.toggled=!0,this.refresh()},refresh:function(){var a=this,b=a.base.toThemeProperty("jqx-fill-state-disabled"),c=a.base.toThemeProperty("jqx-fill-state-normal");a.base.enableDefault||(c="");var d=a.base.toThemeProperty("jqx-fill-state-hover"),e=a.base.toThemeProperty("jqx-fill-state-pressed"),f=a.base.toThemeProperty("jqx-fill-state-pressed"),g="";return a.base.element.disabled=a.base.disabled,a.base.disabled?(g=c+" "+b,void a.base.buttonObj.addClass(g)):(g=a.base.isMouseOver&&!a.isTouchDevice?a.base.isPressed||a.toggled?f:d:a.base.isPressed||a.toggled?e:c,"default"!==a.base.template&&""!==a.base.template&&(g+=" jqx-"+a.base.template,""!=a.base.theme&&(g+=" jqx-"+a.template+"-"+a.base.theme)),a.base.buttonObj.hasClass(b)&&b!=g&&a.base.buttonObj.removeClass(b),a.base.buttonObj.hasClass(c)&&c!=g&&a.base.buttonObj.removeClass(c),a.base.buttonObj.hasClass(d)&&d!=g&&a.base.buttonObj.removeClass(d),a.base.buttonObj.hasClass(e)&&e!=g&&a.base.buttonObj.removeClass(e),a.base.buttonObj.hasClass(f)&&f!=g&&a.base.buttonObj.removeClass(f),void(a.base.buttonObj.hasClass(g)||a.base.buttonObj.addClass(g)))}})}(jqxBaseFramework);

