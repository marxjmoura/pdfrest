import $ from 'jquery'
import hljs from 'highlight.js/lib/highlight'
import cs from 'highlight.js/lib/languages/cs'
import js from 'highlight.js/lib/languages/javascript'
import json from 'highlight.js/lib/languages/json'

hljs.registerLanguage('cs', cs)
hljs.registerLanguage('js', js)
hljs.registerLanguage('json', json)

$('pre code').each(function (i, code) {
  hljs.highlightBlock(code);
});
