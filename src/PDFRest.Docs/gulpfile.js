const gulp = require('gulp')
const autoprefixer = require('gulp-autoprefixer')
const babelify = require('babelify')
const bro = require('gulp-bro')
const connect = require('gulp-connect')
const htmlmin = require('gulp-htmlmin')
const rename = require("gulp-rename")
const sass = require('gulp-sass')
const uglify = require('gulp-uglify')
const handlebars = require('./handlebars')

gulp.task('build-sass', () => {
  return gulp.src('www/scss/docs.scss')
    .pipe(sass({ outputStyle: 'compressed' }).on('error', (e) => console.log(e)))
    .pipe(autoprefixer({ overrideBrowserslist: ['last 2 versions'], cascade: false }))
    .pipe(rename({ extname: '.min.css' }))
    .pipe(gulp.dest('dist'))
})

gulp.task('build-js', () => {
  return gulp.src('www/js/docs.js')
    .pipe(bro({ transform: [babelify.configure({ presets: ['@babel/preset-env'] })] }))
    .pipe(uglify().on('error', (e) => console.log(e)))
    .pipe(rename({ extname: '.min.js' }))
    .pipe(gulp.dest('dist'))
})

gulp.task('build-html', () => {
  return gulp.src(['www/**/_*.hbs', 'www/**/*.hbs'], { base: './www' })
    .pipe(handlebars())
    .pipe(rename({ extname: '.html' }))
    .pipe(htmlmin({ collapseWhitespace: true })
      .on('error', (e) => console.log(e)))
    .pipe(gulp.dest('dist'))
})

gulp.task('copy-images', () => {
  return gulp.src(['www/**/*.{svg,jpg,png,ico}', '!www/**/_*.*'])
    .pipe(gulp.dest('dist'))
})

gulp.task('copy-fonts', () => {
  return gulp.src('www/**/*.ttf')
    .pipe(gulp.dest('dist'))
})

gulp.task('build', gulp.parallel('build-html', 'build-sass', 'build-js', 'copy-images', 'copy-fonts'))

gulp.task('watch', done => {
  gulp.watch('www/**/*.scss', gulp.series('build-sass'))
  gulp.watch('www/**/*.js', gulp.series('build-js'))
  gulp.watch('www/**/*.hbs', gulp.series('build-html'))
  gulp.watch('www/**/*.{svg,jpg,png,ico}', gulp.series('copy-images'))
  gulp.watch('www/**/*.ttf', gulp.series('copy-fonts'))

  done()
})

gulp.task('serve', done => {
  connect.server({ root: 'dist', port: 8888, fallback: 'dist/404.html' })
  connect.serverClose()

  done()
})

gulp.task('start', done => {
  process.env.NODE_ENV = 'development'
  gulp.series('serve', 'watch', 'build')(done)
})

gulp.task('publish', done => {
  process.env.NODE_ENV = 'production'
  gulp.series('build')(done)
})
