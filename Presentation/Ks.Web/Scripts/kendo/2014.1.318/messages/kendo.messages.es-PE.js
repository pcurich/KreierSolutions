(function ($, undefined) {
/* FlatColorPicker messages */

if (kendo.ui.FlatColorPicker) {
kendo.ui.FlatColorPicker.prototype.options.messages =
$.extend(true, kendo.ui.FlatColorPicker.prototype.options.messages,{
  "apply": "Aplicar",
  "cancel": "Cancelar"
});
}

/* ColorPicker messages */

if (kendo.ui.ColorPicker) {
kendo.ui.ColorPicker.prototype.options.messages =
$.extend(true, kendo.ui.ColorPicker.prototype.options.messages,{
  "apply": "Aplicar",
  "cancel": "Cancelar"
});
}

/* ColumnMenu messages */

if (kendo.ui.ColumnMenu) {
kendo.ui.ColumnMenu.prototype.options.messages =
$.extend(true, kendo.ui.ColumnMenu.prototype.options.messages,{
  "sortAscending": "Orden ascendente",
  "sortDescending": "Orden descendente",
  "filter": "Filtros",
  "columns": "Columnas",
  "done": "Hecho",
  "settings": "Configuraci�n de columnas",
  "lock": "Bloquear",
  "unlock": "Desbloquear"
});
}

/* Editor messages */

if (kendo.ui.Editor) {
kendo.ui.Editor.prototype.options.messages =
$.extend(true, kendo.ui.Editor.prototype.options.messages,{
  "bold": "Negrita",
  "italic": "It�lica",
  "underline": "Subrayado",
  "strikethrough": "Tachado",
  "superscript": "Super�ndice",
  "subscript": "Sub�ndice",
  "justifyCenter": "Texto centrado",
  "justifyLeft": "Alinear texto a la izquierda",
  "justifyRight": "Alinear texto a la derecha",
  "justifyFull": "Justificar",
  "insertUnorderedList": "Insertar lista desordenada",
  "insertOrderedList": "Insertar lista ordenada",
  "indent": "Poner sangr�a",
  "outdent": "Quitar sangr�a",
  "createLink": "Insert enlace",
  "unlink": "Eliminar enlace",
  "insertImage": "Insertar imagen",
  "insertFile": "Insertar fichero",
  "insertHtml": "Insertar HTML",
  "viewHtml": "Ver HTML",
  "fontName": "Seleccionar familia de fuentes",
  "fontNameInherit": "(fuente heredada)",
  "fontSize": "Seleccionar tama�o de fuente",
  "fontSizeInherit": "(tama�o heredado)",
  "formatBlock": "Formato",
  "formatting": "Formato",
  "foreColor": "Color",
  "backColor": "Color de fondo",
  "style": "Estilos",
  "emptyFolder": "Carpeta vac�a",
  "uploadFile": "Subir",
  "orderBy": "Ordenados por:",
  "orderBySize": "Tama�o",
  "orderByName": "Nombre",
  "invalidFileType": "El fichero seleccionado \"{0}\" no es v�lido. Los tipos de ficheros soportados son {1}.",
  "deleteFile": '�Est� seguro que quiere eliminar "{0}"?',
  "overwriteFile": 'Un fichero con el nombre "{0}" ya existe en el directorio actual. �Desea reemplazarlo?',
  "directoryNotFound": "Un directorio con este nombre no fue encontrado.",
  "imageWebAddress": "Direcci�n Web",
  "imageAltText": "Texto alternativo",
  "imageWidth": "Ancho (px)",
  "imageHeight": "Alto (px)",
  "fileWebAddress": "Direcci�n Web",
  "fileTitle": "T�tulo",
  "linkWebAddress": "Direcci�n Web",
  "linkText": "Texto",
  "linkToolTip": "ToolTip",
  "linkOpenInNewWindow": "Abrir enlace en nueva ventana",
  "dialogUpdate": "Actualizar",
  "dialogInsert": "Insertar",
  "dialogButtonSeparator": "o",
  "dialogCancel": "Cancelar",
  "createTable": "Crear tabla",
  "addColumnLeft": "Agregar columna a la izquierda",
  "addColumnRight": "Agregar columna a la derecha",
  "addRowAbove": "Agregar fila arriba",
  "addRowBelow": "Agregar fila abajo",
  "deleteRow": "Borrar fila",
  "deleteColumn": "Borrar columna"
});
}

/* FileBrowser messages */

if (kendo.ui.FileBrowser) {
kendo.ui.FileBrowser.prototype.options.messages =
$.extend(true, kendo.ui.FileBrowser.prototype.options.messages,{
  "uploadFile": "Subir fichero",
  "orderBy": "Ordenar por",
  "orderByName": "Nombre",
  "orderBySize": "Tama�o",
  "directoryNotFound": "Un directorio con este nombre no fue encontrado.",
  "emptyFolder": "Carpeta vac�a",
  "deleteFile": '�Est� seguro que quiere eliminar "{0}"?',
  "invalidFileType": "El fichero seleccionado \"{0}\" no es v�lido. Los tipos de ficheros soportados son {1}.",
  "overwriteFile": "Un fichero con el nombre \"{0}\" ya existe en el directorio actual. �Desea reemplazarlo?",
  "dropFilesHere": "arrastre un fichero aqu� para subir",
  "search": "Buscar"
});
}

/* FilterCell messages */

if (kendo.ui.FilterCell) {
kendo.ui.FilterCell.prototype.options.messages =
$.extend(true, kendo.ui.FilterCell.prototype.options.messages,{
  "isTrue": "S�",
  "isFalse": "No",
  "filter": "Filtrar",
  "clear": "Limpiar filtro",
  "operator": "Operador"
});
}

/* FilterCell operators */

if (kendo.ui.FilterCell) {
kendo.ui.FilterCell.prototype.options.operators =
$.extend(true, kendo.ui.FilterCell.prototype.options.operators,{
  "string": {
    "eq": "Es igual a",
    "neq": "No es igual a",
    "startswith": "Comienza con",
    "contains": "Contiene",
    "doesnotcontain": "No contiene",
    "endswith": "Termina en",
    "isnull": "Es nulo",
    "isnotnull": "No es nulo",
    "isempty": "Est� vac�o",
    "isnotempty": "No est� vac�o"
  },
  "number": {
    "eq": "Es igual a",
    "neq": "No es igual a",
    "gte": "Es mayor o igual que",
    "gt": "Es mayor que",
    "lte": "Es menor o igual que",
    "lt": "Es menor que",
    "isnull": "Es nulo",
    "isnotnull": "No es nulo"
  },
  "date": {
    "eq": "Es igual a",
    "neq": "No es igual a",
    "gte": "Es posterior o igual a",
    "gt": "Es posterior",
    "lte": "Es anterior o igual a",
    "lt": "Es anterior",
    "isnull": "Es nulo",
    "isnotnull": "No es nulo"
  },
  "enums": {
    "eq": "Es igual a",
    "neq": "No es igual a",
    "isnull": "Es nulo",
    "isnotnull": "No es nulo"
  }
});
}

/* FilterMenu messages */

if (kendo.ui.FilterMenu) {
kendo.ui.FilterMenu.prototype.options.messages =
$.extend(true, kendo.ui.FilterMenu.prototype.options.messages,{
  "info": "Mostrar filas con valor que:",
  "isTrue": "S�",
  "isFalse": "No",
  "filter": "Filtrar",
  "clear": "Limpiar filtros",
  "and": "Y",
  "or": "O",
  "selectValue": "-Seleccionar valor -",
  "operator": "Operador",
  "value": "Valor",
  "cancel": "Cancelar"
});
}

/* FilterMenu operator messages */

if (kendo.ui.FilterMenu) {
kendo.ui.FilterMenu.prototype.options.operators =
$.extend(true, kendo.ui.FilterMenu.prototype.options.operators,{
  "string": {
    "eq": "Es igual a",
    "neq": "No es igual a",
    "startswith": "Comienza con",
    "contains": "Contiene",
    "doesnotcontain": "No contiene",
    "endswith": "Termina en",
    "isnull": "Es nulo",
    "isnotnull": "No es nulo",
    "isempty": "Est� vac�o",
    "isnotempty": "No est� vac�o"
  },
  "number": {
    "eq": "Es igual a",
    "neq": "No es igual a",
    "gte": "Es mayor o igual que",
    "gt": "Es mayor que",
    "lte": "Es menor o igual que",
    "lt": "Es menor que",
    "isnull": "Es nulo",
    "isnotnull": "No es nulo"
  },
  "date": {
    "eq": "Es igual a",
    "neq": "Es diferente a",
    "gte": "Es posterior o igual a",
    "gt": "Es posterior",
    "lte": "Es anterior o igual a",
    "lt": "Es anterior",
    "isnull": "Es nulo",
    "isnotnull": "No es nulo"
  },
  "enums": {
    "eq": "Es igual a",
    "neq": "No es igual a",
    "isnull": "Es nulo",
    "isnotnull": "No es nulo"
  }
});
}

/* FilterMultiCheck messages */

if (kendo.ui.FilterMultiCheck) {
kendo.ui.FilterMultiCheck.prototype.options.messages =
$.extend(true, kendo.ui.FilterMultiCheck.prototype.options.messages,{
  "checkAll": "Seleccionar todo",
  "clear": "Limpiar filtros",
  "filter": "Filtrar",
  "search": "Buscar"
});
}

/* Gantt messages */

if (kendo.ui.Gantt) {
kendo.ui.Gantt.prototype.options.messages =
$.extend(true, kendo.ui.Gantt.prototype.options.messages,{
  "actions": {
    "addChild": "Agregar sub-tarea",
    "append": "Agregar tarea",
    "insertAfter": "Insertar abajo",
    "insertBefore": "Insertar arriba",
    "pdf": "Exportar a PDF"
  },
  "cancel": "Cancelar",
  "deleteDependencyWindowTitle": "Borrar dependencia",
  "deleteTaskWindowTitle": "Borrar tarea",
  "destroy": "Borrar",
  "editor": {
    "assingButton": "Asignar",
    "editorTitle": "Tarea",
    "end": "Fin",
    "percentComplete": "Completa",
    "resources": "Recursos",
    "resourcesEditorTitle": "Recursos",
    "resourcesHeader": "Recursos",
    "start": "Comienzo",
    "title": "T�tulo",
    "unitsHeader": "Unidades"
  },
  "save": "Guardar",
  "views": {
    "day": "D�a",
    "end": "Fin",
    "month": "Mes",
    "start": "Comienzo",
    "week": "Semana",
    "year": "A�o"
  }
});
}

/* Grid messages */

if (kendo.ui.Grid) {
kendo.ui.Grid.prototype.options.messages =
$.extend(true, kendo.ui.Grid.prototype.options.messages,{
  "commands": {
    "cancel": "Cancelar Cambios",
    "canceledit": "Cancelar",
    "create": "Agregar",
    "destroy": "Eliminar",
    "edit": "Editar",
    "excel": "Exportar a Excel",
    "pdf": "Exportar a PDF",
    "save": "Guardar Cambios",
    "select": "Seleccionar",
    "update": "Actualizar"
  },
  "editable": {
    "cancelDelete": "Cancelar",
    "confirmation": "�Confirma la eliminaci�n de este registro?",
    "confirmDelete": "Eliminar"
  },
  "noRecords": "No hay datos disponibles."
});
}

/* Groupable messages */

if (kendo.ui.Groupable) {
kendo.ui.Groupable.prototype.options.messages =
$.extend(true, kendo.ui.Groupable.prototype.options.messages,{
  "empty": "Arrastre el t�tulo de una columna y su�ltelo aqu� para agrupar por ese criterio"
});
}

/* NumericTextBox messages */

if (kendo.ui.NumericTextBox) {
kendo.ui.NumericTextBox.prototype.options =
$.extend(true, kendo.ui.NumericTextBox.prototype.options,{
  "upArrowText": "Incrementar valor",
  "downArrowText": "Disminuir valor"
});
}

/* Pager messages */

if (kendo.ui.Pager) {
kendo.ui.Pager.prototype.options.messages =
$.extend(true, kendo.ui.Pager.prototype.options.messages,{
  "allPages": "Todas",
  "display": "Elementos mostrados {0} - {1} de {2}",
  "empty": "No hay registros.",
  "page": "P�gina",
  "of": "de {0}",
  "itemsPerPage": "�tems por p�gina",
  "first": "Ir a la primera p�gina",
  "previous": "Ir a la p�gina anterior",
  "next": "Ir a la p�gina siguiente",
  "last": "Ir a la �ltima p�gina",
  "refresh": "Actualizar",
  "morePages": "M�s paginas"
});
}

/* RecurrenceEditor messages */

if (kendo.ui.RecurrenceEditor) {
kendo.ui.RecurrenceEditor.prototype.options.messages =
$.extend(true, kendo.ui.RecurrenceEditor.prototype.options.messages,{
  "frequencies": {
    "never": "Nunca",
    "hourly": "Por hora",
    "daily": "Diariamente",
    "weekly": "Semanalmente",
    "monthly": "Mensualmente",
    "yearly": "Anualmente"
  },
  "hourly": {
    "repeatEvery": "Repetir cada: ",
    "interval": " hora(s)"
  },
  "daily": {
    "repeatEvery": "Repetir cada: ",
    "interval": " d�a(s)"
  },
  "weekly": {
    "interval": " semana(s)",
    "repeatEvery": "Repetir cada: ",
    "repeatOn": "Repetir en: "
  },
  "monthly": {
    "repeatEvery": "Repetir cada: ",
    "repeatOn": "Repetir en: ",
    "interval": " mes(es)",
    "day": "D�a "
  },
  "yearly": {
    "repeatEvery": "Repetir cada: ",
    "repeatOn": "Repetir en: ",
    "interval": " a�o(s)",
    "of": " de "
  },
  "end": {
    "label": "Fin:",
    "mobileLabel": "Fin",
    "never": "Nunca",
    "after": "Despu�s",
    "occurrence": " ocurrencia(s)",
    "on": "En "
  },
  "offsetPositions": {
    "first": "primero",
    "second": "segundo",
    "third": "tercero",
    "fourth": "cuarto",
    "last": "�ltimo"
  },
  "weekdays": {
    "day": "d�a",
    "weekday": "d�a de semana",
    "weekend": "d�a de fin de semana"
  }
});
}

/* Scheduler messages */

if (kendo.ui.Scheduler) {
kendo.ui.Scheduler.prototype.options.messages =
$.extend(true, kendo.ui.Scheduler.prototype.options.messages,{
  "allDay": "todo el d�a",
  "date": "Fecha",
  "event": "Evento",
  "time": "Hora",
  "showFullDay": "Mostrar d�a completo",
  "showWorkDay": "Mostrar horas laborables",
  "today": "Hoy",
  "save": "Guardar",
  "cancel": "Cancelar",
  "destroy": "Eliminar",
  "deleteWindowTitle": "Eliminar evento",
  "ariaSlotLabel": "Seleccionado desde {0:t} hasta {1:t}",
  "ariaEventLabel": "{0} en {1:D} al {2:t}",
  "editable": {
    "confirmation": "�Est� seguro que quiere eliminar este evento?"
  },
  "views": {
    "day": "D�a",
    "week": "Semana",
    "workWeek": "Semana laboral",
    "agenda": "Agenda",
    "month": "Mes"
  },
  "recurrenceMessages": {
    "deleteWindowTitle": "Eliminar elemento recurrente",
    "deleteWindowOccurrence": "Eliminar ocurrencia actual",
    "deleteWindowSeries": "Eliminar la serie",
    "editWindowTitle": "Editar elemento recurrente",
    "editWindowOccurrence": "Editar ocurrencia actual",
    "editWindowSeries": "Editar la serie",
    "deleteRecurring": "�Quiere eliminar esta ocurrencia del evento o la serie completa?",
    "editRecurring": "�Quiere editar esta ocurrencia del evento o la serie completa?"
  },
  "editor": {
    "title": "T�tulo",
    "start": "Inicio",
    "end": "Fin",
    "allDayEvent": "Todo el d�a",
    "description": "Descripci�n",
    "repeat": "Repetir",
    "timezone": " ",
    "startTimezone": "Zona horaria de inicio",
    "endTimezone": "Zona horaria de fin",
    "separateTimezones": "Usar zonas horarias separadas para el inicio y el fin",
    "timezoneEditorTitle": "Zonas horarias",
    "timezoneEditorButton": "Zona horaria",
    "noTimezone": "Sin zona horaria",
    "editorTitle": "Evento"
  }
});
}

/* Slider messages */

if (kendo.ui.Slider) {
kendo.ui.Slider.prototype.options =
$.extend(true, kendo.ui.Slider.prototype.options,{
  "increaseButtonTitle": "Aumentar",
  "decreaseButtonTitle": "Disminuir"
});
}

/* TreeView messages */

if (kendo.ui.TreeView) {
kendo.ui.TreeView.prototype.options.messages =
$.extend(true, kendo.ui.TreeView.prototype.options.messages,{
  "loading": "Cargando...",
  "requestFailed": "Fallo en solicitud.",
  "retry": "Reintentar"
});
}

/* Upload messages */

if (kendo.ui.Upload) {
kendo.ui.Upload.prototype.options.localization =
$.extend(true, kendo.ui.Upload.prototype.options.localization,{
  "select": "Seleccione...",
  "cancel": "Cancelar",
  "retry": "Reintentar",
  "remove": "Quitar",
  "uploadSelectedFiles": "Subir archivos",
  "dropFilesHere": "Arrastre los archivos aqu� para subirlos",
  "statusUploading": "subiendo",
  "statusUploaded": "Completado",
  "statusWarning": "advertencia",
  "statusFailed": "Error",
  "headerStatusUploading": "Subiendo...",
  "headerStatusUploaded": "Completado"
});
}

/* Validator messages */

if (kendo.ui.Validator) {
kendo.ui.Validator.prototype.options.messages =
$.extend(true, kendo.ui.Validator.prototype.options.messages,{
  "required": "{0} es requerido",
  "pattern": "{0} no es v�lido",
  "min": "{0} debe ser mayor o igual a {1}",
  "max": "{0} debe ser menor o igual a {1}",
  "step": "{0} no es v�lido",
  "email": "{0} no es un correo electr�nico v�lido",
  "url": "{0} no es una URL v�lida",
  "date": "{0} no es una fecha v�lida",
  "dateCompare": "Fecha final debe ser mayor o igual a fecha inicial"
});
}

/* Dialog */

if (kendo.ui.Dialog) {
kendo.ui.Dialog.prototype.options.messages =
$.extend(true, kendo.ui.Dialog.prototype.options.localization, {
  "close": "Cerca"
});
}

/* Alert */

if (kendo.ui.Alert) {
kendo.ui.Alert.prototype.options.messages =
$.extend(true, kendo.ui.Alert.prototype.options.localization, {
  "okText": "OK"
});
}

/* Confirm */

if (kendo.ui.Confirm) {
kendo.ui.Confirm.prototype.options.messages =
$.extend(true, kendo.ui.Confirm.prototype.options.localization, {
  "okText": "OK",
  "cancel": "Cancelar"
});
}

/* Prompt */
if (kendo.ui.Prompt) {
kendo.ui.Prompt.prototype.options.messages =
$.extend(true, kendo.ui.Prompt.prototype.options.localization, {
  "okText": "OK",
  "cancel": "Cancelar"
});
}

})(window.kendo.jQuery);