const express = require('express');
const router = express.Router();
const passport = require('passport');
const jwt = require('jsonwebtoken');
const config = require('../config/database');
const User = require('../models/user');
const nodemailer = require('nodemailer');

//setting up nodemailer
var transporter = nodemailer.createTransport({
  service: 'gmail',
  auth: {
    user: 'rowdycoder019@gmail.com',
    pass: 'ellelvethA123#'
  }
});



// Register
router.post('/register', (req, res, next) => {
  User.checkUserExist(req.body,(isTrue)=>{
    console.log(isTrue);

    if (isTrue) {
        res.json({success: false, msg:'Username Aready taken'});
    }

    else {
      User.checkEmailExist(req.body,(isTrue)=>{

        if (isTrue) {
            res.json({success: false, msg:'Already an account linked with this email'});
        }
        else{

var randomNumber = Math.floor(Math.random() *200000*100000*200000*100000*200000*100000);
      let newUser = new User({
        name: req.body.name,
        email: req.body.email,
        username: req.body.username,
        password: req.body.password,
        validation: false,
        random: randomNumber
      });

      User.addUser(newUser, (err, user) => {
        if(err){
          res.json({success: false, msg:'Failed to register user'});
        } else {
          res.json({success: true, msg:'User registered'});


//mail content

      var mailOptions = {
        from: 'vetha.gnanam98@gmail.com',
        to: newUser.email,
        subject: 'Verification of your account',
        text: 'Click on this link to verify your account https://dry-cove-59464.herokuapp.com/users/validate?token='+user.random+'    this is required for your login'
      };

//sending mail

      transporter.sendMail(mailOptions, function(error, info){
  if (error) {
    console.log(error);
  } else {
    console.log('Email sent: ' + info.response);
  }
});
    }
  });
}
})
}
});
});

// Authenticate
router.post('/authenticate', (req, res, next) => {
  const username = req.body.username;
  const password = req.body.password;
  User.getUserByUsername(username, (err, user) => {
    if(err) throw err;
    if(!user){
      return res.json({success: false, msg: 'User not found'});
    }
    User.comparePassword(password, user.password, (err, isMatch) => {
      if(err) throw err;
      if(isMatch){
        if(user.validation==true){
        const token = jwt.sign(user, config.secret, {
          expiresIn: 604800 // 1 week
        });
        res.json({
          success: true,
          token: 'JWT '+token,
          user: {
            id: user._id,
            name: user.name,
            username: user.username,
            email: user.email,

          }
        });
      }else {
          return res.json({success: false, msg: 'Please verify your account'});
      }
      } else {
        return res.json({success: false, msg: 'Wrong password'});
      }
    });
  });
});
// Profile
router.get('/profile', passport.authenticate('jwt', {session:false}), (req, res, next) => {
  res.json(req.user);
});

router.get('/validate', (req, res, next) => {
console.log(req.query.token);
  User.Validate(req.query,(err,cs)=>{
  console.log(err);
      return res.json({msg:"successs"})
 });
});

router.put('/update', function(req, res) {

User.Update(req.body.id,req.body.details,(err,msg)=>{
if(err) throw err;
  return res.json({msg: "success"});
})
});

module.exports = router;
