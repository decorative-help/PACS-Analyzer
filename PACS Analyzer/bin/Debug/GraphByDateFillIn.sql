TRUNCATE TABLE [graphbydate] 

DECLARE @minTimeDiff INT-- choose minimum time for [duration]
DECLARE @numberOfLines INT

DECLARE parent_table_cursor CURSOR -- ОБЬЯВЛЯЕМ КУРСОР  
FOR 
  SELECT start_time, 
         end_time, 
         [user_id], 
         [floor], 
         [zone], 
         duration 
  FROM   [intervals]
  ORDER BY [start_time]

OPEN parent_table_cursor -- ОТКРЫВАЕМ КУРСОР  

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
      -- CHILD - START  
      DECLARE child_table_cursor CURSOR -- ОБЬЯВЛЯЕМ КУРСОР  
      FOR 
        SELECT start_time, 
               end_time, 
               [user_id], 
               [floor], 
               [zone], 
               duration 
        FROM   [intervals] 
        WHERE  [start_time] >= @parent_start_time 
               AND [start_time] <= @parent_end_time 
               AND [floor] = @parent_floor 
               AND [zone] = @parent_zone 
               AND [user_id] <> @parent_user_id 
        ORDER  BY [start_time] 

      OPEN child_table_cursor -- ОТКРЫВАЕМ КУРСОР  

      -- КУРСОР СОЗДАН, ОБЬЯВЛЯЕМ ПЕРЕМЕННЫЕ И ОБХОДИМ НАБОР СТРОК В ЦИКЛЕ  
      DECLARE @child_COUNTER    INT, 
              @child_loop       INT, 
              @child_start_time DATETIME, 
              @child_end_time   DATETIME, 
              @child_user_id    INT, 
              @child_floor      INT, 
              @child_zone       VARCHAR(50), 
              @child_duration   INT 

      SET @child_COUNTER = 0 

      -- ВЫБОРКА ПЕРВОЙ  СТРОКИ  
      FETCH next FROM child_table_cursor INTO @child_start_time, @child_end_time 
      , 
      @child_user_id, @child_floor, @child_zone, @child_duration 

      -- ЦИКЛ С ЛОГИКОЙ И ВЫБОРКОЙ ВСЕХ ПОСЛЕДУЮЩИХ СТРОК ПОСЛЕ ПЕРВОЙ  
      SET @child_loop = @@FETCH_STATUS 

      WHILE @child_loop = 0 -- read [intervals]Child line by line   
        BEGIN -- for each user who has been there  
            -- CHILD - FINISH
            IF EXISTS (SELECT * 
                       FROM   [graphbydate] 
                       WHERE  [user_source_id] <> [user_target_id] 
                              AND (
								([user_source_id] = @parent_user_id 
                                    AND [user_target_id] = @child_user_id ) 
								OR ( [user_source_id] = @child_user_id 
                                    AND [user_target_id] = @parent_user_id )
								)
                                  AND [date] = CONVERT(date, @parent_start_time) 
                                  AND [zone] = @parent_zone 
                                  AND [floor] = @parent_floor) 
              BEGIN -- line already exists, then UPDATE  
                  -- retrieve data for next INSERT || UPDATE queries first
                  DECLARE @graph_duration INT, 
                          @graph_times    INT 

                  SELECT @graph_duration = duration, 
                         @graph_times = times 
                  FROM   [graphbydate] 
                  WHERE  [user_source_id] <> [user_target_id] 
                         AND (
								([user_source_id] = @parent_user_id 
                                    AND [user_target_id] = @child_user_id ) 
								OR ( [user_source_id] = @child_user_id 
                                    AND [user_target_id] = @parent_user_id )
								) 
                             AND [date] = CONVERT(date, @parent_start_time)
                             AND [zone] = @parent_zone 
                             AND [floor] = @parent_floor 
                  ORDER  BY [date]

                  IF ( Datediff(minute, @child_start_time, @parent_end_time) >= 
                          Datediff(minute, @child_start_time, @child_end_time) ) 
                    SET @minTimeDiff = Datediff(minute, @child_start_time, 
                                       @child_end_time) 
                  ELSE 
                    SET @minTimeDiff = Datediff(minute, @child_start_time, 
                                       @parent_end_time) 

                  BEGIN try-- try to UPDATE  
                      UPDATE [graphbydate] 
                      SET    [duration] = @graph_duration 
                                          + @minTimeDiff, 
                             [times] = @graph_times + 1 
                      WHERE  [user_source_id] <> [user_target_id] 
                             AND (
								([user_source_id] = @parent_user_id 
                                    AND [user_target_id] = @child_user_id ) 
								OR ( [user_source_id] = @child_user_id 
                                    AND [user_target_id] = @parent_user_id )
								)
                                 AND [date] = CONVERT(date, @parent_start_time)
                                 AND [zone] = @parent_zone 
                                 AND [floor] = @parent_floor 
                  END try 

                  BEGIN catch 
                      -- ...UPDATE or nothing  
                      PRINT 'Cannot UPDATE [GraphByDate]' 
                  END catch 
              END-- of IF EXISTS  
            ELSE 
              BEGIN -- there is no such lines, then INSERT 

                  IF ( Datediff(minute, @child_start_time, @parent_end_time) >= 
                          Datediff(minute, @child_start_time, @child_end_time) ) 
                    SET @minTimeDiff = Datediff(minute, @child_start_time, 
                                       @child_end_time) 
                  ELSE 
                    SET @minTimeDiff = Datediff(minute, @child_start_time, 
                                       @parent_end_time) 

                  BEGIN try-- try to INSERT  				  
			SET @numberOfLines = @numberOfLines + 1
                      INSERT INTO [graphbydate] 
                                  ([date], 
                                   user_source_id, 
                                   user_target_id, 
                                   duration, 
                                   times, 
                                   [zone], 
                                   [floor]) 
                      VALUES      (CONVERT(date, @parent_start_time), 
                                   @parent_user_id, 
                                   @child_user_id, 
                                   @minTimeDiff, 
                                   1, 
                                   @parent_zone, 
                                   @parent_floor) 
                  END try 

                  BEGIN catch 
                      -- ...UPDATE or nothing  
                      PRINT 'Cannot INSERT INTO [GraphByDate]' 
                  END catch 
              END-- IF EXISTS  

            --!!!!!YOUR CODE GOES HERE!!!!!!!!  
            -- CHILD - FINISH  
            -- ВЫБОРКА СЛЕДУЮЩЕЙ СТРОКИ  
            FETCH next FROM child_table_cursor INTO @child_start_time, 
            @child_end_time 
            , 
            @child_user_id, @child_floor, @child_zone, @child_duration 

            SET @child_loop = @@FETCH_STATUS 
        -- ЗАВЕРШЕНИЕ ЛОГИКИ ВНУТРИ ЦИКЛА  
        END 

      --SELECT @child_COUNTER AS FINAL_COUNT  
      -- ЗАКРЫВАЕМ КУРСОР  
      CLOSE child_table_cursor 

      DEALLOCATE child_table_cursor 

      -- CHILD - FINISH  
      SET @parent_COUNTER = @parent_COUNTER + 1 

      -- ВЫБОРКА СЛЕДУЮЩЕЙ СТРОКИ  
      FETCH next FROM parent_table_cursor INTO @parent_start_time, 
      @parent_end_time, @parent_user_id, @parent_floor, @parent_zone, 
      @parent_duration 

      SET @parent_loop = @@FETCH_STATUS 
  -- ЗАВЕРШЕНИЕ ЛОГИКИ ВНУТРИ ЦИКЛА  
  END 

-- ЗАКРЫВАЕМ КУРСОР  
CLOSE parent_table_cursor 

DEALLOCATE parent_table_cursor











 


	  /*
	  https://habrahabr.ru/post/305926/
	  https://habrahabr.ru/post/27439/
	  https://metanit.com/sql/sqlserver/10.4.php
	  https://stackoverflow.com/questions/41015307/sql-server-if-exists-then-1-else-2/41015373
	  http://nullpro.info/2012/vypolnyaem-kod-dlya-kazhdoj-stroki-v-vyborke-transact-sql/
	  http://www.dpriver.com/pp/sqlformat.htm
	  https://www.sqlservercentral.com/Forums/FindPost81097.aspx


	  */
