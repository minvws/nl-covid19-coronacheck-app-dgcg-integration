-- Remove existing verifier config
delete from public."AppConfig" where type='verifier';

-- Add verifier config
insert into public."AppConfig" (content, type)
select
'{"androidMinimumVersion":1,"androidMinimumVersionMessage":"Om de app te gebruiken heb je de laatste versie uit de store nodig.","playStoreURL":"https://www.google.com","iosMinimumVersion":"1.0.0","iosMinimumVersionMessage":"Om de app te gebruiken heb je de laatste versie uit de store nodig.","iosAppStoreURL":"https://apps.apple.com/nl/app/id1548269870","appDeactivated":false,"informationURL":"https://coronatester.nl"}' as content,
'verifier' as type;

-- Remove existing holder config
delete from public."AppConfig"  where type='holder';

-- Add holder config
insert into public."AppConfig" (content, type)
select
'{"proofOfTestValidity":180,"androidMinimumVersion":1,"androidMinimumVersionMessage":"Om de app te gebruiken heb je de laatste versie uit de store nodig.","playStoreURL":"https://www.google.com","iosMinimumVersion":"1.0.0","iosMinimumVersionMessage":"Om de app te gebruiken heb je de laatste versie uit de store nodig.","iosAppStoreURL":"https://apps.apple.com/nl/app/id1548269870","appDeactivated":false,"informationURL":"https://coronatester.nl"}' as content,
'holder' as type;