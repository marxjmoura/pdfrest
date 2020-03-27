import $ from 'jquery'

const $body = $('body')
const $nav = $('nav')

$('.nav-toggle').on('click', function (e) {
  $nav.scrollTop(0)

  const toggle = !$body.hasClass('nav-show')
  $body.toggleClass('nav-show', toggle)

  e.preventDefault()
})
