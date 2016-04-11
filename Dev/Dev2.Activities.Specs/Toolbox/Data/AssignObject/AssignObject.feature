﻿Feature: AssignObject
	In order to use json 
	As a Warewolf user
	I want a tool that assigns data to json objects

Scenario: Assign a value to a json object
	Given I assign the value "Bob" to a json object "[[Person.Name]]"
	When the assign object tool is executed
	Then the json object "[[Person.Name]]" equals "Bob"
	And the execution has "NO" error
	And the debug inputs as
	| # | Variable			| New Value		|
	| 1 | [[Person.Name]] = | Bob			|
	And the debug output as
	| # |						|
	| 1 | [[Person.Name]] = Bob |

Scenario: Assign values to json objects
	Given I assign the value "Bob" to a json object "[[Person.FirstName]]"
	And I assign the value "Smith" to a json object "[[Person.Surname]]"
	When the assign object tool is executed
	Then the json object "[[Person.FirstName]]" equals "Bob"
	And the json object "[[Person.Surname]]" equals "Smith"
	And the execution has "NO" error
	And the debug inputs as
	| # | Variable					| New Value	|
	| 1 | [[Person.FirstName]] =	| Bob		|
	| 2 | [[Person.Surname]] =		| Smith		|
	And the debug output as
	| # |								|
	| 1 | [[Person.FirstName]] = Bob	|
	| 2 | [[Person.Surname]] = Smith	|

Scenario: Assign values with different types to json objects
	Given I assign the value "Bob" to a json object "[[Person.FirstName]]"
	And I assign the value "Smith" to a json object "[[Person.Surname]]"
	And I assign the value "21" to a json object "[[Person.Age]]"
	When the assign object tool is executed
	Then the json object "[[Person.FirstName]]" equals "Bob"
	And the json object "[[Person.Surname]]" equals "Smith"
	And the json object "[[Person.Age]]" equals "21"
	And the execution has "NO" error
	And the debug inputs as
	| # | Variable					| New Value	|
	| 1 | [[Person.FirstName]] =	| Bob		|
	| 2 | [[Person.Surname]] =		| Smith		|
	| 3 | [[Person.Age]] =			| 21		|
	And the debug output as
	| # |								|
	| 1 | [[Person.FirstName]] = Bob	|
	| 2 | [[Person.Surname]] = Smith	|
	| 3 | [[Person.Age]] = 21		|

#failing - debug output
Scenario: Assign a json object value to a json object
	Given I assign the value "Bob" to a json object "[[Person.FirstName]]"
	And I assign the value "[[Person.FirstName]]" to a json object "[[Person.Surname]]"
	When the assign object tool is executed
	Then the json object "[[Person.FirstName]]" equals "Bob"
	And the json object "[[Person.Surname]]" equals "Bob"
	And the execution has "NO" error
	And the debug inputs as
	| # | Variable					| New Value	|
	| 1 | [[Person.FirstName]] =	| Bob		|
	| 2 | [[Person.Surname]] =		| Bob		|
	And the debug output as
	| # |								|
	| 1 | [[Person.FirstName]] = Bob	|
	| 2 | [[Person.Surname]] = Bob		|

#failing - debug output
Scenario: Assign a json object value to a json object overwriting the existing value
	Given I assign the value "Bob" to a json object "[[Person.FirstName]]"
	And I assign the value "Smith" to a json object "[[Person.Surname]]"
	And I assign the value "[[Person.FirstName]]" to a json object "[[Person.Surname]]"
	When the assign object tool is executed
	Then the json object "[[Person.FirstName]]" equals "Bob"
	And the json object "[[Person.Surname]]" equals "Bob"
	And the execution has "NO" error
	And the debug inputs as
	| # | Variable						| New Value						|
	| 1 | [[Person.FirstName]] =		| Bob							|
	| 2 | [[Person.Surname]] =			| Smith							|
	| 3 | [[Person.Surname]] = Smith	| [[Person.FirstName]] = Bob	|
	And the debug output as
	| # |								|
	| 1 | [[Person.FirstName]] = Bob	|
	| 2 | [[Person.Surname]] = Smith	|
	| 3 | [[Person.FirstName]] = Bob	|

Scenario: Assign a value to an invalid json object
	Given I assign the value "[[Person.Score]]" to a json object "[[Person..Score]]"
	When the assign object tool is executed
	Then the execution has "AN" error
	And the execution has "parse error" error

Scenario: Assign an invalid value to a json object
	Given I assign the value "[[Person..Score]]" to a json object "[[Person..Score]]"
	When the assign object tool is executed
	Then the execution has "AN" error
	And the execution has "parse error" error

Scenario: Assign a value with plus in it to a json object
	Given I assign the value "+10" to a json object "[[Person.Score]]"
	When the assign object tool is executed
	Then the value of "[[Person.Score]]" equals +10
	And the execution has "NO" error
	And the debug inputs as
	| # | Variable  | New Value	|
	| 1 | [[Person.Score]] =	| +10       |
	And the debug output as 
	| # |							|
	| 1 | [[Person.Score]] = +10	|

Scenario: Assign a value with minus in it to a json object
	Given I assign the value "-10" to a json object "[[Person.Score]]"
	When the assign object tool is executed
	Then the value of "[[Person.Score]]" equals -10
	And the execution has "NO" error
	And the debug inputs as
	| # | Variable  | New Value	|
	| 1 | [[Person.Score]] =	| -10       |
	And the debug output as 
	| # |							|
	| 1 | [[Person.Score]] = -10	|

#failing
# person.name = bob, person.age = 25, staff = person -> is this valid?
Scenario: Assign a populated json object to a new json object
	Given I assign the value "Bob" to a json object "[[Person.FirstName]]"
	And I assign the value "Smith" to a json object "[[Person.Surname]]"
	And I assign the json object "[[Person]]" to a json object "[[Staff]]"
	When the assign object tool is executed
	Then the json object "[[Staff.FirstName]]" equals "Bob"
	Then the json object "[[Staff.Surname]]" equals "Smith"
	And the execution has "NO" error
	And the debug inputs as
	| # | Variable						| New Value						|
	| 1 | [[Person.FirstName]] =		| Bob							|
	| 2 | [[Person.Surname]] =			| Smith							|
	| 3 | [[Person]] = 					| [[Staff]]						|
	And the debug output as
	| # |								|
	| 1 | [[Person.FirstName]] = Bob	|
	| 2 | [[Person.Surname]] = Smith	|
	| 3 | [[Staff.FirstName]] = Bob		|
	| 4 | [[Staff.Surname]] = Smith		|

#failing
# person.name = bob, person.age = 25, staff.subordinate = person -> is this valid?
Scenario: Assign a populated json object to a child of a new json object
	Given I assign the value "Bob" to a json object "[[Person.FirstName]]"
	And I assign the value "Smith" to a json object "[[Person.Surname]]"
	And I assign the json object "[[Person]]" to a json object "[[Staff.Subordinate]]"
	When the assign object tool is executed
	Then the json object "[[Staff.Subordinate.FirstName]]" equals "Bob"
	Then the json object "[[Staff.Subordinate.Surname]]" equals "Smith"
	And the execution has "NO" error
	And the debug inputs as
	| # | Variable						| New Value						|
	| 1 | [[Person.FirstName]] =		| Bob							|
	| 2 | [[Person.Surname]] =			| Smith							|
	| 3 | [[Person]] = 					| [[Staff.Subordinate]]			|
	And the debug output as
	| # |											|
	| 1 | [[Person.FirstName]] = Bob				|
	| 2 | [[Person.Surname]] = Smith				|
	| 3 | [[Staff.Subordinate.FirstName]] = Bob		|
	| 4 | [[StaffSubordinate..Surname]] = Smith		|

#failing
Scenario: Assign multiple json variables to a json object
	Given I assign the value "Bob" to a json object "[[Person.FirstName]]"
	And I assign the value "Smith" to a json object "[[Person.Surname]]"
	And I assign the value "[[Person.FirstName]][[Person.Surname]]" to a json object "[[Person.FullName]]"
	When the assign tool is executed
	Then the json object "[[Person.FirstName]]" equals "Bob"
	And the json object "[[Person.Surname]]" equals "Smith"
	And the value of "[[Person.FullName]]" equals BobSmith
	And the execution has "NO" error
	And the debug inputs as
	| # | Variable						| New Value									|
	| 1 | [[Person.FirstName]] =		| Bob										|
	| 2 | [[Person.Surname]] =			| Smith										|
	| 3 | [[Person.FullName]] =			| [[Person.FirstName]][[Person.Surname]]	|
	And the debug output as
    | # |									|
    | 1 | [[Person.FirstPart]] = Bob		|
    | 2 | [[Person.SecondPart]] = Smith		|
    | 3 | [[Person.FullName]]  = BobSmith	|

#failing - causes exception in IntellisenseStringProvider.fs
Scenario: Assign multiple json variables to a json object with a literal
	Given I assign the value "Bob" to a json object "[[Person.FirstName]]"
	And I assign the value "Smith" to a json object "[[Person.Surname]]"
	And I assign the value "[[Person.FirstName]] the killa [[Person.Surname]]" to a json object "[[Person.FullName]]"
	When the assign tool is executed
	Then the json object "[[Person.FirstName]]" equals "Bob"
	And the json object "[[Person.Surname]]" equals "Smith"
	And the value of "[[Person.FullName]]" equals BobSmith
	And the execution has "NO" error
	And the debug inputs as
	| # | Variable						| New Value									|
	| 1 | [[Person.FirstName]] =		| Bob										|
	| 2 | [[Person.Surname]] =			| Smith										|
	| 3 | [[Person.FullName]] =			| [[Person.FirstName]][[Person.Surname]]	|
	And the debug output as
    | # |									|
    | 1 | [[Person.FirstPart]] = Bob		|
    | 2 | [[Person.SecondPart]] = Smith		|
    | 3 | [[Person.FullName]]  = BobSmith	|

# TODO
# do we want calculate , v2?
# add to the seq, foreach specs, adding this tool
# debug output -> json 
