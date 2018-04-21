DECLARE anomalies_cursor CURSOR -- ОБЬЯВЛЯЕМ КУРСОР  
FOR 
  SELECT 
	[date]
	,user_source_id
	,user_target_id
	,duration
	,times
	,[zone]
	,[floor] 
  FROM [graphbydate]
  ORDER BY [date]

OPEN anomalies_cursor -- ОТКРЫВАЕМ КУРСОР  

-- КУРСОР СОЗДАН, ОБЬЯВЛЯЕМ ПЕРЕМЕННЫЕ И ОБХОДИМ НАБОР СТРОК В ЦИКЛЕ  
DECLARE @parent_COUNTER    INT, 
        @parent_loop       INT, 
        @parent_start_time DATETIME, 
        @parent_end_time   DATETIME, 
        @parent_user_id    INT, 
        @parent_floor      INT, 
        @parent_zone       VARCHAR(50), 
        @parent_duration   INT 

SET @parent_COUNTER = 0 

-- ВЫБОРКА ПЕРВОЙ  СТРОКИ  
FETCH next FROM parent_table_cursor INTO @parent_start_time, @parent_end_time, 
@parent_user_id, @parent_floor, @parent_zone, @parent_duration 

-- ЦИКЛ С ЛОГИКОЙ И ВЫБОРКОЙ ВСЕХ ПОСЛЕДУЮЩИХ СТРОК ПОСЛЕ ПЕРВОЙ  
SET @parent_loop = @@FETCH_STATUS 

WHILE @parent_loop = 0 
  BEGIN 

  /*

select AVG(duration) AS 'duration', AVG(times) as 'times'
from GraphByDate
where user_source_id = 2 OR user_target_id = 2
group by [floor], [zone]





Надо вообще свой лог сделать, потому что ладно три дня обрабатывается, но у них и встреча одна...(
Надо самому и сделать